using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourismPlatform.Core.DTOs.Tenant;
using TourismPlatform.Data.Interfaces;

namespace TourismPlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TenantController : ControllerBase
    {
        private readonly ITenantService _tenantService;
        public TenantController(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }

        [HttpGet("{subdomain}")]
        public async Task<ActionResult<TenantDto>> GetTenantBySubdomain(string subdomain)
        {
            try
            {
                var tenant = await _tenantService.GetBySubdomainAsync(subdomain);

                if (tenant == null)
                    return NotFound(new { message = "Tenant no encontrado" });

                var tenantDto = new TenantDto
                {
                    Id = tenant.Id.ToString(),
                    Name = tenant.Name,
                    Subdomain = tenant.Subdomain,
                    IsActive = tenant.IsActive,
                    CreatedAt = tenant.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
                };

                return Ok(tenantDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}