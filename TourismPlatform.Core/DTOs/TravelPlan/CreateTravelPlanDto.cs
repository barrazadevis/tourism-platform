using System;
using System.ComponentModel.DataAnnotations;

namespace TourismPlatform.Core.DTOs.TravelPlan;

public class CreateTravelPlanDto
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(200)]
    public string Destination { get; set; } = string.Empty;
    
    [Required]
    [Range(1, 365)]
    public int DurationDays { get; set; }
    
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal BasePrice { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string PlanType { get; set; } = string.Empty;
    
    public string Inclusions { get; set; } = string.Empty;
    public string Exclusions { get; set; } = string.Empty;
    
    public List<CreatePlanServiceDto> Services { get; set; } = new();
}