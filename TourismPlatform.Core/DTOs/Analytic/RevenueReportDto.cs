using System;

namespace TourismPlatform.Core.DTOs.Analytic;

public class RevenueReportDto
{
    public DateTime Date { get; set; }
    public decimal DailyRevenue { get; set; }
    public int DailyBookings { get; set; }
    public decimal MonthToDateRevenue { get; set; }
    public decimal YearToDateRevenue { get; set; }
}