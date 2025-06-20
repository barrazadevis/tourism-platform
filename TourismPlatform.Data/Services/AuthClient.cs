using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using TourismPlatform.Data.Interfaces;

namespace TourismPlatform.Data.Services;
public class AuthClient : IAuthClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public AuthClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<AuthValidationResult> ValidateTokenAsync(string token)
    {
        try
        {
            var authApiUrl = _configuration["AuthService:BaseUrl"];
            var clientId = _configuration["AuthService:ClientId"];

            var request = new
            {
                Token = token,
                ClientId = clientId
            };

            var response = await _httpClient.PostAsJsonAsync($"{authApiUrl}/api/auth/validate", request);
            var result = await response.Content.ReadFromJsonAsync<TokenValidationResponse>();

            if (result?.IsValid == true && result.User != null)
            {
                return new AuthValidationResult
                {
                    IsValid = true,
                    UserId = result.User.Id,
                    UserName = result.User.Name,
                    UserEmail = result.User.Email,
                    Role = result.User.Role.ToString(),
                    TenantId = result.User.Tenant.Id
                };
            }

            return new AuthValidationResult
            {
                IsValid = false,
                Error = result?.Error ?? "Invalid token"
            };
        }
        catch (Exception ex)
        {
            return new AuthValidationResult
            {
                IsValid = false,
                Error = ex.Message
            };
        }
    }
}