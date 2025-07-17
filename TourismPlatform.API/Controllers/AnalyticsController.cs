using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourismPlatform.API.Middleware;
using TourismPlatform.Core.DTOs.Analytic;
using TourismPlatform.Data.Interfaces;

namespace TourismPlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AnalyticsController : BaseController
    {
        private readonly IAnalyticsService _analyticsService;

        public AnalyticsController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        [HttpGet("dashboard")]
        public async Task<ActionResult<DashboardStatsDto>> GetDashboardStats()
        {
            var tenantId = GetTenantId();
            var stats = await _analyticsService.GetDashboardStatsAsync(tenantId);
            return Ok(stats);
        }

        [HttpGet("revenue")]
        public async Task<ActionResult<List<RevenueReportDto>>> GetRevenueReport(
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate)
        {
            var tenantId = GetTenantId();
            var report = await _analyticsService.GetRevenueReportAsync(tenantId, fromDate, toDate);
            return Ok(report);
        }

        [HttpGet("top-customers")]
        public async Task<ActionResult<List<TopCustomersDto>>> GetTopCustomers([FromQuery] int limit = 10)
        {
            var tenantId = GetTenantId();
            var customers = await _analyticsService.GetTopCustomersAsync(tenantId, limit);
            return Ok(customers);
        }

        [HttpGet("popular-destinations")]
        public async Task<ActionResult<List<PopularDestinationsDto>>> GetPopularDestinations([FromQuery] int limit = 10)
        {
            var tenantId = GetTenantId();
            var destinations = await _analyticsService.GetPopularDestinationsAsync(tenantId, limit);
            return Ok(destinations);
        }

        [HttpGet("monthly-metrics")]
        public async Task<ActionResult<List<MonthlyMetricsDto>>> GetMonthlyMetrics([FromQuery] int months = 12)
        {
            var tenantId = GetTenantId();
            var metrics = await _analyticsService.GetMonthlyMetricsAsync(tenantId, months);
            return Ok(metrics);
        }

        [HttpPost("generate-monthly-metrics")]
        public async Task<ActionResult> GenerateMonthlyMetrics([FromQuery] DateTime? month = null)
        {
            try
            {
                var tenantId = GetTenantId();
                await _analyticsService.GenerateMonthlyMetricsAsync(tenantId, month);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error generating metrics: {ex.Message}");
            }
        }
    }
}