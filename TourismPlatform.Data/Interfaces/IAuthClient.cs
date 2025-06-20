namespace TourismPlatform.Data.Interfaces;
public interface IAuthClient
{
    Task<AuthValidationResult> ValidateTokenAsync(string token);
}
