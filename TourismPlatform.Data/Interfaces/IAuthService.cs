using TourismPlatform.Core.DTOs;
using TourismPlatform.Core.Entities;

namespace TourismPlatform.Data.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginRequestDto request, string tenantSubdomain);
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, string tenantSubdomain);
        Task<User?> GetUserFromTokenAsync(string token);
        Task<bool> ValidateTokenAsync(string token);
    }
}
