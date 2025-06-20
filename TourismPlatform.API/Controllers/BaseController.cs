using Microsoft.AspNetCore.Mvc;
using TourismPlatform.Core.Entities;

namespace TourismPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{
    protected Tenant? CurrentTenant => HttpContext.Items["Tenant"] as Tenant;
    protected Guid TenantId => CurrentTenant?.Id ?? Guid.Empty;
    
    // Auth context
    protected Guid? AuthenticatedUserId => 
        HttpContext.Items["UserId"] as Guid?;
    protected string? AuthenticatedUserName => 
        HttpContext.Items["UserName"] as string;
    protected string? AuthenticatedUserEmail => 
        HttpContext.Items["UserEmail"] as string;
    protected string? AuthenticatedUserRole => 
        HttpContext.Items["UserRole"] as string;
    protected Guid? AuthenticatedTenantId => 
        HttpContext.Items["AuthenticatedTenantId"] as Guid?;

    protected IActionResult TenantNotFound()
    {
        return BadRequest(new { error = "Tenant not found or invalid" });
    }

    protected IActionResult Unauthorized()
    {
        return StatusCode(401, new { error = "Authentication required" });
    }

    protected bool IsValidTenant()
    {
        return CurrentTenant != null && TenantId != Guid.Empty;
    }

    protected bool IsAuthenticated()
    {
        return AuthenticatedUserId.HasValue;
    }

    protected bool HasTenantAccess()
    {
        return IsAuthenticated() && AuthenticatedTenantId == TenantId;
    }
}