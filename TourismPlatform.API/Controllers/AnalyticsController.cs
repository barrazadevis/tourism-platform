using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            var stats = await _analyticsService.GetDashboardStatsAsync();
            return Ok(stats);
        }

        [HttpGet("revenue")]
        public async Task<ActionResult<List<RevenueReportDto>>> GetRevenueReport(
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate)
        {
            var report = await _analyticsService.GetRevenueReportAsync(fromDate, toDate);
            return Ok(report);
        }

        [HttpGet("top-customers")]
        public async Task<ActionResult<List<TopCustomersDto>>> GetTopCustomers([FromQuery] int limit = 10)
        {
            var customers = await _analyticsService.GetTopCustomersAsync(limit);
            return Ok(customers);
        }

        [HttpGet("popular-destinations")]
        public async Task<ActionResult<List<PopularDestinationsDto>>> GetPopularDestinations([FromQuery] int limit = 10)
        {
            var destinations = await _analyticsService.GetPopularDestinationsAsync(limit);
            return Ok(destinations);
        }

        [HttpGet("monthly-metrics")]
        public async Task<ActionResult<List<MonthlyMetricsDto>>> GetMonthlyMetrics([FromQuery] int months = 12)
        {
            var metrics = await _analyticsService.GetMonthlyMetricsAsync(months);
            return Ok(metrics);
        }

        [HttpPost("generate-monthly-metrics")]
        public async Task<ActionResult> GenerateMonthlyMetrics([FromQuery] DateTime? month = null)
        {
            try
            {
                await _analyticsService.GenerateMonthlyMetricsAsync(month);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error generating metrics: {ex.Message}");
            }
        }
    }
}