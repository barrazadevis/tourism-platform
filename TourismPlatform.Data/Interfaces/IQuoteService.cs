using TourismPlatform.Core.Entities;
namespace TourismPlatform.Data.Interfaces;

public interface IQuoteService
{
    Task<List<Quote>> GetByTenantAsync(Guid tenantId);
    Task<Quote?> GetByIdAsync(Guid id, Guid tenantId);
    Task<Quote> CreateAsync(Quote quote);
    Task<Quote> UpdateAsync(Quote quote);
    Task<bool> DeleteAsync(Guid id, Guid tenantId);
}