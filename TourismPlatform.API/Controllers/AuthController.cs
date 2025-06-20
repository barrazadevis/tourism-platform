using Microsoft.AspNetCore.Mvc;

namespace TourismPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public AuthController(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] object loginRequest)
    {
        var authApiUrl = _configuration["AuthService:BaseUrl"];
        var response = await _httpClient.PostAsJsonAsync($"{authApiUrl}/api/auth/login", loginRequest);
        var content = await response.Content.ReadAsStringAsync();
        
        return StatusCode((int)response.StatusCode, content);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] object registerRequest)
    {
        var authApiUrl = _configuration["AuthService:BaseUrl"];
        var response = await _httpClient.PostAsJsonAsync($"{authApiUrl}/api/auth/register", registerRequest);
        var content = await response.Content.ReadAsStringAsync();
        
        return StatusCode((int)response.StatusCode, content);
    }
}