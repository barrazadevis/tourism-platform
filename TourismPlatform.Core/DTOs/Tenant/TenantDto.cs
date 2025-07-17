namespace TourismPlatform.Core.DTOs.Tenant;
public class TenantDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Subdomain { get; set; }
    public bool IsActive { get; set; }
    public string CreatedAt { get; set; }
}