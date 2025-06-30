using System;

namespace TourismPlatform.Core.DTOs.Analytic;

public class PopularDestinationsDto
{
    public string Destination { get; set; } = string.Empty;
    public int BookingCount { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AveragePrice { get; set; }
}
