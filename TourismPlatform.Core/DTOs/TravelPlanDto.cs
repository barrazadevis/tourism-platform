using System.ComponentModel.DataAnnotations;
using TourismPlatform.Core.Entities;

namespace TourismPlatform.Core.DTOs;

public class CreateTravelPlanDto
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    [Required, Range(0.01, double.MaxValue)]
    public decimal BasePrice { get; set; }
    
    [Range(1, 365)]
    public int DurationDays { get; set; }
    
    public List<string> Destinations { get; set; } = new();
    public List<string> Services { get; set; } = new();
}

public class UpdateTravelPlanDto : CreateTravelPlanDto
{
    public PlanStatus Status { get; set; }
}

public class TravelPlanResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal BasePrice { get; set; }
    public int DurationDays { get; set; }
    public List<string> Destinations { get; set; } = [];
    public List<string> Services { get; set; } = [];
    public PlanStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}