using System;
using System.ComponentModel.DataAnnotations;
using TourismPlatform.Core.DTOs.Destination;

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
    public Guid DestinationId { get; set; } = Guid.NewGuid();

    [Required]
    [Range(1, 365)]
    public int DurationDays { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal BasePrice { get; set; }

    public string[] Inclusions { get; set; } = Array.Empty<string>();
    public string[] Exclusions { get; set; } = Array.Empty<string>();

    public List<CreatePlanServiceDto> Services { get; set; } = new();
    public DestinationDto? Destination { get; set; }
    
}