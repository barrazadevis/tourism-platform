using Microsoft.AspNetCore.Mvc;
using TourismPlatform.Core.Entities;
using TourismPlatform.Data.Interfaces;
namespace TourismPlatform.API.Controllers;

public class DashboardController : BaseController
{
    private readonly IQuoteService _quoteService;
    private readonly ITravelPlanService _travelPlanService;

    public DashboardController(IQuoteService quoteService, ITravelPlanService travelPlanService)
    {
        _quoteService = quoteService;
        _travelPlanService = travelPlanService;
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        if (!IsValidTenant())
        {
            return TenantNotFound();
        }

        var quotes = await _quoteService.GetByTenantAsync(TenantId);
        var plans = await _travelPlanService.GetByTenantAsync(TenantId);

        var stats = new
        {
            TotalQuotes = quotes.Count,
            AcceptedQuotes = quotes.Count(q => q.Status == QuoteStatus.Accepted),
            PendingQuotes = quotes.Count(q => q.Status == QuoteStatus.Sent),
            TotalRevenue = quotes
                .Where(q => q.Status == QuoteStatus.Accepted)
                .Sum(q => q.TotalAmount),
            ActivePlans = plans.Count(p => p.Status == PlanStatus.Active),
            ConversionRate = quotes.Count > 0
                ? Math.Round((double)quotes.Count(q => q.Status == QuoteStatus.Accepted) / quotes.Count * 100, 2)
                : 0,
            MonthlyQuotes = quotes
                .Where(q => q.CreatedAt >= DateTime.UtcNow.AddMonths(-1))
                .Count(),
            AvgQuoteValue = quotes.Count > 0
                ? Math.Round(quotes.Average(q => q.TotalAmount), 2)
                : 0
        };

        return Ok(stats);
    }
}