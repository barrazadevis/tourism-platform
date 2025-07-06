using Microsoft.EntityFrameworkCore;
using TourismPlatform.Core.DTOs.Analytic;
using TourismPlatform.Core.Entities;
using TourismPlatform.Core.Enums;
using TourismPlatform.Data.Common;
using TourismPlatform.Data.Interfaces;

namespace TourismPlatform.Data.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly TourismDbContext _context;

    public AnalyticsService(TourismDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardStatsDto> GetDashboardStatsAsync(Guid tenantId)
    {
        var now = DateTime.UtcNow;
        var thisMonth = new DateTime(now.Year, now.Month, 1);
        DateTimeUtils.EnsureUtcDateTimes(thisMonth);

        var totalCustomers = await _context.Customers.CountAsync(c => c.TenantId == tenantId);
        var totalQuotes = await _context.Quotes.CountAsync(q => q.TenantId == tenantId);
        var totalBookings = await _context.Bookings.CountAsync(b => b.TenantId == tenantId);
        
        var totalRevenue = await _context.Bookings
            .Where(b => b.TenantId == tenantId && b.Status == BookingStatus.Completed)
            .SumAsync(b => b.TotalPaid);

        var pendingPayments = await _context.Bookings
            .Where(b => b.TenantId == tenantId && b.PaymentStatus != PaymentStatus.Paid)
            .SumAsync(b => b.PendingAmount);

        var newCustomersThisMonth = await _context.Customers
            .CountAsync(c => c.TenantId == tenantId && c.CreatedAt.ToUniversalTime() >= thisMonth.ToUniversalTime());

        var quotesThisMonth = await _context.Quotes
            .CountAsync(q => q.TenantId == tenantId && q.CreatedAt.ToUniversalTime() >= thisMonth.ToUniversalTime());

        var bookingsThisMonth = await _context.Bookings
            .CountAsync(b => b.TenantId == tenantId && b.CreatedAt.ToUniversalTime() >= thisMonth.ToUniversalTime());

        var revenueThisMonth = await _context.Bookings
            .Where(b => b.TenantId == tenantId && b.CreatedAt.ToUniversalTime() >= thisMonth.ToUniversalTime() && b.Status == BookingStatus.Completed)
            .SumAsync(b => b.TotalPaid);

        return new DashboardStatsDto
        {
            TotalCustomers = totalCustomers,
            TotalQuotes = totalQuotes,
            TotalBookings = totalBookings,
            TotalRevenue = totalRevenue,
            PendingPayments = pendingPayments,
            NewCustomersThisMonth = newCustomersThisMonth,
            QuotesThisMonth = quotesThisMonth,
            BookingsThisMonth = bookingsThisMonth,
            RevenueThisMonth = revenueThisMonth
        };
    }

    public async Task<List<RevenueReportDto>> GetRevenueReportAsync(Guid tenantId, DateTime fromDate, DateTime toDate)
    {
        var payments = await _context.Payments
            .Include(p => p.Booking)
            .Where(p => p.TenantId == tenantId && 
                        p.PaymentDate >= fromDate.ToUniversalTime() && 
                        p.PaymentDate <= toDate.ToUniversalTime() &&
                        p.PaymentStatus == PaymentStatus.Paid)
            .GroupBy(p => p.PaymentDate.Date)
            .Select(g => new RevenueReportDto
            {
                Date = g.Key,
                DailyRevenue = g.Sum(p => p.Amount),
                DailyBookings = g.Select(p => p.BookingId).Distinct().Count()
            })
            .OrderBy(r => r.Date)
            .ToListAsync();

        // Calculate MTD and YTD for each day
        foreach (var report in payments)
        {
            var monthStart = new DateTime(report.Date.Year, report.Date.Month, 1);
            var yearStart = new DateTime(report.Date.Year, 1, 1);

            report.MonthToDateRevenue = await _context.Payments
                .Where(p => p.TenantId == tenantId && 
                            p.PaymentDate >= monthStart && 
                            p.PaymentDate <= report.Date &&
                            p.PaymentStatus == PaymentStatus.Paid)
                .SumAsync(p => p.Amount);

            report.YearToDateRevenue = await _context.Payments
                .Where(p => p.TenantId == tenantId && 
                            p.PaymentDate >= yearStart && 
                            p.PaymentDate <= report.Date &&
                            p.PaymentStatus == PaymentStatus.Paid)
                .SumAsync(p => p.Amount);
        }

        return payments;
    }

    public async Task<List<TopCustomersDto>> GetTopCustomersAsync(Guid tenantId, int limit = 10)
    {
        return await _context.Customers
            .Where(c => c.TenantId == tenantId)
            .Include(c => c.Bookings)
            .Select(c => new TopCustomersDto
            {
                CustomerId = c.Id,
                CustomerName = c.FirstName + " " + c.LastName,
                Email = c.Email,
                TotalBookings = c.Bookings.Count,
                TotalSpent = c.Bookings.Where(b => b.Status == BookingStatus.Completed).Sum(b => b.TotalPaid),
                LastBookingDate = c.Bookings.OrderByDescending(b => b.BookingDate).Select(b => b.BookingDate).FirstOrDefault()
            })
            .Where(c => c.TotalBookings > 0)
            .OrderByDescending(c => c.TotalSpent)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<List<PopularDestinationsDto>> GetPopularDestinationsAsync(Guid tenantId, int limit = 10)
    {
        return await _context.Bookings
            .Include(b => b.Quote)
                .ThenInclude(q => q.TravelPlan)
            .Where(b => b.TenantId == tenantId && b.Quote.TravelPlan != null)
            .GroupBy(b => b.Quote.TravelPlan!.Destination)
            .Select(g => new PopularDestinationsDto
            {
                Destination = g.Key,
                BookingCount = g.Count(),
                TotalRevenue = g.Where(b => b.Status == BookingStatus.Completed).Sum(b => b.TotalPaid),
                AveragePrice = g.Where(b => b.Status == BookingStatus.Completed).Average(b => b.TotalPaid)
            })
            .OrderByDescending(d => d.BookingCount)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<List<MonthlyMetricsDto>> GetMonthlyMetricsAsync(Guid tenantId, int months = 12)
    {
        return await _context.BookingMetrics
            .Where(bm => bm.TenantId == tenantId)
            .OrderByDescending(bm => bm.ReportDate)
            .Take(months)
            .Select(bm => new MonthlyMetricsDto
            {
                Year = bm.ReportDate.Year,
                Month = bm.ReportDate.Month,
                MonthName = bm.ReportDate.ToString("MMMM yyyy"),
                TotalBookings = bm.TotalBookings,
                TotalRevenue = bm.TotalRevenue,
                NewCustomers = bm.NewCustomers,
                CancelledBookings = bm.CancelledBookings,
                AverageBookingValue = bm.AverageBookingValue
            })
            .ToListAsync();
    }

    public async Task GenerateMonthlyMetricsAsync(Guid tenantId, DateTime? month = null)
    {
        var targetMonth = month ?? DateTime.UtcNow.AddMonths(-1);
        var monthStart = new DateTime(targetMonth.Year, targetMonth.Month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);

        var totalBookings = await _context.Bookings
            .CountAsync(b => b.TenantId == tenantId && 
                            b.BookingDate >= monthStart && 
                            b.BookingDate <= monthEnd);

        var totalRevenue = await _context.Bookings
            .Where(b => b.TenantId == tenantId && 
                        b.BookingDate >= monthStart && 
                        b.BookingDate <= monthEnd &&
                        b.Status == BookingStatus.Completed)
            .SumAsync(b => b.TotalPaid);

        var newCustomers = await _context.Customers
            .CountAsync(c => c.TenantId == tenantId &&
                            c.CreatedAt >= monthStart &&
                            c.CreatedAt <= monthEnd);

        var cancelledBookings = await _context.Bookings
            .CountAsync(b => b.TenantId == tenantId &&
                            b.BookingDate >= monthStart &&
                            b.BookingDate <= monthEnd &&
                            b.Status == BookingStatus.Cancelled);

        var averageBookingValue = totalBookings > 0 ? totalRevenue / totalBookings : 0;

        var existingMetric = await _context.BookingMetrics
            .FirstOrDefaultAsync(bm => bm.TenantId == tenantId && 
                                        bm.ReportDate.Year == targetMonth.Year &&
                                        bm.ReportDate.Month == targetMonth.Month);

        if (existingMetric != null)
        {
            existingMetric.TotalBookings = totalBookings;
            existingMetric.TotalRevenue = totalRevenue;
            existingMetric.NewCustomers = newCustomers;
            existingMetric.CancelledBookings = cancelledBookings;
            existingMetric.AverageBookingValue = averageBookingValue;
        }
        else
        {
            var newMetric = new BookingMetric
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                ReportDate = monthStart,
                TotalBookings = totalBookings,
                TotalRevenue = totalRevenue,
                NewCustomers = newCustomers,
                CancelledBookings = cancelledBookings,
                AverageBookingValue = averageBookingValue,
                Period = "monthly"
            };

            _context.BookingMetrics.Add(newMetric);
        }

        await _context.SaveChangesAsync();
    }
}