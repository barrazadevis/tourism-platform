using TourismPlatform.Core.Entities;
using TourismPlatform.Core.Enums;

public class UserInfo
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public TenantInfo Tenant { get; set; } = null!;
}