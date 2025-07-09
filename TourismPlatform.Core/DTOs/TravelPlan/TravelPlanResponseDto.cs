using System;

namespace TourismPlatform.Core.DTOs.TravelPlan;

public class TravelPlanResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid DestinationId { get; set; } = Guid.NewGuid();
    public int DurationDays { get; set; }
    public decimal BasePrice { get; set; }
    public string PlanType { get; set; } = string.Empty;
    public string Inclusions { get; set; } = string.Empty;
    public string Exclusions { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    public List<PlanServiceResponseDto> Services { get; set; } = new();
    public int TotalQuotes { get; set; }
    public decimal TotalPrice { get; set; }
}
