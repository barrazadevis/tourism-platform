using System;

namespace TourismPlatform.Core.DTOs.Analytic;

public class DashboardStatsDto
{
    public int TotalCustomers { get; set; }
    public int TotalQuotes { get; set; }
    public int TotalBookings { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal PendingPayments { get; set; }
    public int NewCustomersThisMonth { get; set; }
    public int QuotesThisMonth { get; set; }
    public int BookingsThisMonth { get; set; }
    public decimal RevenueThisMonth { get; set; }
}
