using TourismPlatform.Data.Interfaces;

namespace TourismPlatform.API.Middleware;

public class AuthMiddleware
{
    private readonly RequestDelegate _next;

    public AuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IAuthClient authClient)
    {
        // Skip auth for certain paths
        var path = context.Request.Path.Value?.ToLower();
        if (path != null && (path.Contains("/swagger") || path == "/" || path.Contains("/health")))
        {
            await _next(context);
            return;
        }

        var token = ExtractTokenFromHeader(context);
        if (!string.IsNullOrEmpty(token))
        {
            var validationResult = await authClient.ValidateTokenAsync(token);
            if (validationResult.IsValid)
            {
                context.Items["UserId"] = validationResult.UserId;
                context.Items["UserName"] = validationResult.UserName;
                context.Items["UserEmail"] = validationResult.UserEmail;
                context.Items["UserRole"] = validationResult.Role;
                context.Items["AuthenticatedTenantId"] = validationResult.TenantId;
            }
            else
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }
        }
        else if (RequiresAuth(context))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Token required");
            return;
        }

        await _next(context);
    }

    private string? ExtractTokenFromHeader(HttpContext context)
    {
        var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
        if (authHeader?.StartsWith("Bearer ") == true)
        {
            return authHeader["Bearer ".Length..];
        }
        return null;
    }

    private bool RequiresAuth(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLower();
        return path != null && (
            path.Contains("/api/travelplans") ||
            path.Contains("/api/quotes") ||
            path.Contains("/api/dashboard")
        );
    }
}