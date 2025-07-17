using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TourismPlatform.Core.DTOs;
using TourismPlatform.Data.Interfaces;
using TourismPlatform.Data.Services;

namespace TourismPlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ITenantService _tenantService;

        public AuthController(IAuthService authService, ITenantService tenantService)
        {
            _authService = authService;
            _tenantService = tenantService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                var tenant = await _tenantService.GetBySubdomainAsync(request.TenantSubdomain);
                if (tenant == null)
                    return BadRequest("Tenant no encontrado. Verifique la URL.");

                var response = await _authService.LoginAsync(request, tenant.Subdomain);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterRequestDto request)
        {
            try
            {
                var tenant = await _tenantService.GetBySubdomainAsync(request.TenantSubdomain);
                if (tenant == null)
                    return BadRequest(new { message = "Tenant no encontrado. Verifique la URL." });

                // Verificar que el tenant esté activo
                if (!tenant.IsActive)
                    return BadRequest(new { message = "El tenant no está activo para registros." });

                var response = await _authService.RegisterAsync(request, tenant.Subdomain);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}