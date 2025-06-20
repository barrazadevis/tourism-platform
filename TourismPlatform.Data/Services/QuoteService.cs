using Microsoft.EntityFrameworkCore;
using TourismPlatform.Core.Entities;
using TourismPlatform.Data.Interfaces;

namespace TourismPlatform.Data.Services;
public class QuoteService : IQuoteService
{
    private readonly TourismDbContext _context;

    public QuoteService(TourismDbContext context)
    {
        _context = context;
    }

    public async Task<List<Quote>> GetByTenantAsync(Guid tenantId)
    {
        return await _context.Quotes
            .Include(q => q.TravelPlan)
            .Where(q => q.TenantId == tenantId)
            .OrderByDescending(q => q.CreatedAt)
            .ToListAsync();
    }

    public async Task<Quote?> GetByIdAsync(Guid id, Guid tenantId)
    {
        return await _context.Quotes
            .Include(q => q.TravelPlan)
            .FirstOrDefaultAsync(q => q.Id == id && q.TenantId == tenantId);
    }

    public async Task<Quote> CreateAsync(Quote quote)
    {
        quote.Id = Guid.NewGuid();
        quote.QuoteNumber = GenerateQuoteNumber();
        quote.CreatedAt = DateTime.UtcNow;
        quote.ExpiresAt = DateTime.UtcNow.AddDays(30); // 30 días de validez
        
        _context.Quotes.Add(quote);
        await _context.SaveChangesAsync();
        return quote;
    }

    public async Task<Quote> UpdateAsync(Quote quote)
    {
        quote.UpdatedAt = DateTime.UtcNow;
        _context.Quotes.Update(quote);
        await _context.SaveChangesAsync();
        return quote;
    }

    public async Task<bool> DeleteAsync(Guid id, Guid tenantId)
    {
        var quote = await GetByIdAsync(id, tenantId);
        if (quote == null) return false;

        _context.Quotes.Remove(quote);
        await _context.SaveChangesAsync();
        return true;
    }

    private string GenerateQuoteNumber()
    {
        return $"COT-{DateTime.UtcNow:yyyyMM}-{Random.Shared.Next(1000, 9999)}";
    }
}