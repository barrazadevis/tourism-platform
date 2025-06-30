using System;

namespace TourismPlatform.Core.DTOs.Analytic;

public class MonthlyMetricsDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string MonthName { get; set; } = string.Empty;
    public int TotalBookings { get; set; }
    public decimal TotalRevenue { get; set; }
    public int NewCustomers { get; set; }
    public int CancelledBookings { get; set; }
    public decimal AverageBookingValue { get; set; }
}