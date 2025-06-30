using System;

namespace TourismPlatform.Core.DTOs.TravelPlan;

public class TravelPlanSearchDto
{
    public string? SearchTerm { get; set; }
    public string? Destination { get; set; }
    public string? PlanType { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int? MinDuration { get; set; }
    public int? MaxDuration { get; set; }
    public bool? IsActive { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}