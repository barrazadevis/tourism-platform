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
                var tenant = await _tenantService.GetCurrentTenantAsync();
                if (tenant == null)
                    return BadRequest("Tenant no encontrado");

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
                var tenant = await _tenantService.GetCurrentTenantAsync();
                if (tenant == null)
                    return BadRequest("Tenant no encontrado");

                var response = await _authService.RegisterAsync(request, tenant.Subdomain);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPost("validate")]
        public async Task<ActionResult> ValidateToken()
        {
            try
            {
                var token = HttpContext.Request.Headers["token"].FirstOrDefault();
                if (string.IsNullOrEmpty((string?)token))
                    return Unauthorized(new { message = "Token inválido" });
                var user = await _authService.GetUserFromTokenAsync(token);
                if (user == null)
                    return Unauthorized(new { message = "Usuario no encontrado" });

                return Ok(new
                {
                    valid = true,
                    user = new UserDto
                    {
                        Id = user.Id,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Role = user.Role,
                        TenantId = user.TenantId,
                        TenantName = user.Tenant.Name,
                        TenantSubdomain = user.Tenant.Subdomain
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    public class ValidateTokenRequest
    {
        public string Token { get; set; } = string.Empty;
    }
}