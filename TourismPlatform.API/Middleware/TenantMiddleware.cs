using TourismPlatform.Data.Interfaces;

namespace TourismPlatform.API.Middleware;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITenantService tenantService)
    {
        var host = context.Request.Host.Host;
        var subdomain = ExtractSubdomain(host);

        if (!string.IsNullOrEmpty(subdomain))
        {
            var tenant = await tenantService.GetBySubdomainAsync(subdomain);
            if (tenant != null)
            {
                context.Items["Tenant"] = tenant;
                context.Items["TenantId"] = tenant.Id;
            }
        }

        await _next(context);
    }

    private string ExtractSubdomain(string host)
    {
        // Para desarrollo local o cuando no hay subdominio
        if (host.Contains("localhost") || !host.Contains("."))
        {
            return "default"; // tenant por defecto para desarrollo
        }

        var parts = host.Split('.');
        return parts.Length > 2 ? parts[0] : string.Empty;
    }
}