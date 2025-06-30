using System;

namespace TourismPlatform.Core.DTOs.TravelPlan;

public class PlanServiceResponseDto
{
    public Guid Id { get; set; }
    public string ServiceType { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsIncluded { get; set; }
    public bool IsOptional { get; set; }
}