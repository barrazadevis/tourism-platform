using System;
using System.ComponentModel.DataAnnotations;

namespace TourismPlatform.Core.DTOs.TravelPlan;

public class CreatePlanServiceDto
{
    [Required]
    [MaxLength(100)]
    public string ServiceType { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }
    
    public bool IsIncluded { get; set; }
    public bool IsOptional { get; set; }
}