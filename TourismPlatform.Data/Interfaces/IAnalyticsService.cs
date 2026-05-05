using System;
using TourismPlatform.Core.DTOs.Analytic;

namespace TourismPlatform.Data.Interfaces;

public interface IAnalyticsService
{
    Task<DashboardStatsDto> GetDashboardStatsAsync();
    Task<List<RevenueReportDto>> GetRevenueReportAsync(DateTime fromDate, DateTime toDate);
    Task<List<TopCustomersDto>> GetTopCustomersAsync(int limit = 10);
    Task<List<PopularDestinationsDto>> GetPopularDestinationsAsync(int limit = 10);
    Task<List<MonthlyMetricsDto>> GetMonthlyMetricsAsync(int months = 12);
    Task GenerateMonthlyMetricsAsync(DateTime? month = null);
}
