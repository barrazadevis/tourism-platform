using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourismPlatform.Core.Entities;

public class TravelPlan
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid DestinationId { get; set; }
    public int DurationDays { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal BasePrice { get; set; }
    public string PlanType { get; set; } = string.Empty;
    public string Inclusions { get; set; } = string.Empty;
    public string Exclusions { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid TenantId { get; set; }
    public Guid CompanyId { get; set; }

    // Navigation properties
    public Tenant Tenant { get; set; } = null!;
    public Company Company { get; set; } = null!;
    public List<PlanService> Services { get; set; } = new();
    public List<Quote> Quotes { get; set; } = new();
    public Destination Destination { get; set; } = null!;
}