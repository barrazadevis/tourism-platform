using TourismPlatform.Core.DTOs.Quote;
namespace TourismPlatform.Data.Interfaces;

public interface IQuoteService
{
    Task<QuoteResponseDto> CreateQuoteAsync(CreateQuoteDto createQuoteDto, Guid tenantId, string createdBy);
    Task<QuoteResponseDto?> GetQuoteByIdAsync(Guid id, Guid tenantId);
    Task<List<QuoteResponseDto>> GetQuotesByTenantAsync(Guid tenantId, int page = 1, int pageSize = 10);
    Task<List<QuoteResponseDto>> GetQuotesByCustomerAsync(Guid customerId, Guid tenantId);
    Task<QuoteResponseDto?> UpdateQuoteAsync(Guid id, CreateQuoteDto updateQuoteDto, Guid tenantId);
    Task<QuoteResponseDto?> UpdateQuoteStatusAsync(Guid id, UpdateQuoteStatusDto statusDto, Guid tenantId);
    Task<bool> DeleteQuoteAsync(Guid id, Guid tenantId);
    Task<string> GenerateQuoteNumberAsync(Guid tenantId);
}