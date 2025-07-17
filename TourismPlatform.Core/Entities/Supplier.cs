using System;

namespace TourismPlatform.Core.Entities;

public class Supplier
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string SupplierType { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public Guid TenantId { get; set; }
    public Guid CompanyId { get; set; }

    // Navigation properties
    public Tenant Tenant { get; set; } = null!;
    public Company Company { get; set; } = null!;
    public List<SupplierService> Services { get; set; } = new();
}
