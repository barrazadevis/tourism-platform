using System.ComponentModel.DataAnnotations.Schema;

namespace TourismPlatform.Core.Entities;

public class PlanService
{
    public Guid Id { get; set; }
    public string ServiceType { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }
    public bool IsIncluded { get; set; }
    public bool IsOptional { get; set; }
    public Guid TravelPlanId { get; set; }

    // Navigation properties
    public TravelPlan TravelPlan { get; set; } = null!;
}
