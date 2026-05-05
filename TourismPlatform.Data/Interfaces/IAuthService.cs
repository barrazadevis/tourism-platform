using TourismPlatform.Core.DTOs;
using TourismPlatform.Core.Entities;

namespace TourismPlatform.Data.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
        Task<User?> GetUserFromTokenAsync(string token);
        Task<bool> ValidateTokenAsync(string token);
    }
}
