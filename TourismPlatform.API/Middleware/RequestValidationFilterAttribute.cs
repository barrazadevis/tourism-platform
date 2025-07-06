using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using TourismPlatform.Data.Services;

namespace TourismPlatform.API.Middleware;

public class RequestValidationFilterAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        SetTokenValidation(context.HttpContext);
        base.OnActionExecuting(context);
    }

    private void SetTokenValidation(HttpContext httpContext)
    {
        var authorizationHeader = httpContext.Request.Headers["Authorization"].ToString();
        if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer "))
        {
            var token = authorizationHeader.Replace("Bearer ", string.Empty);
            if (!string.IsNullOrEmpty(token))
            {
                httpContext.Request.Headers.Append("token", token);
            }
        }

    }
}
