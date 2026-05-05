using TourismPlatform.Core.DTOs.Quote;
namespace TourismPlatform.Data.Interfaces;

public interface IQuoteService
{
    Task<QuoteResponseDto> CreateQuoteAsync(CreateQuoteDto createQuoteDto, string createdBy);
    Task<QuoteResponseDto?> GetQuoteByIdAsync(Guid id);
    Task<List<QuoteResponseDto>> GetQuotesByCompanyAsync(int page = 1, int pageSize = 10);
    Task<List<QuoteResponseDto>> GetQuotesByCustomerAsync(Guid customerId);
    Task<QuoteResponseDto?> UpdateQuoteAsync(Guid id, CreateQuoteDto updateQuoteDto);
    Task<QuoteResponseDto?> UpdateQuoteStatusAsync(Guid id, UpdateQuoteStatusDto statusDto);
    Task<bool> DeleteQuoteAsync(Guid id);
    Task<string> GenerateQuoteNumberAsync();
}