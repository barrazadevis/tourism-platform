using System;
using TourismPlatform.Core.DTOs.Analytic;

namespace TourismPlatform.Data.Interfaces;

public interface IAnalyticsService
{
    Task<DashboardStatsDto> GetDashboardStatsAsync(Guid tenantId);
    Task<List<RevenueReportDto>> GetRevenueReportAsync(Guid tenantId, DateTime fromDate, DateTime toDate);
    Task<List<TopCustomersDto>> GetTopCustomersAsync(Guid tenantId, int limit = 10);
    Task<List<PopularDestinationsDto>> GetPopularDestinationsAsync(Guid tenantId, int limit = 10);
    Task<List<MonthlyMetricsDto>> GetMonthlyMetricsAsync(Guid tenantId, int months = 12);
    Task GenerateMonthlyMetricsAsync(Guid tenantId, DateTime? month = null);
}
