using Microsoft.AspNetCore.Mvc;
using TourismPlatform.Core.DTOs;
using TourismPlatform.Core.Entities;
using TourismPlatform.Data.Interfaces;

namespace TourismPlatform.API.Controllers;

public class QuotesController : BaseController
{
    private readonly IQuoteService _quoteService;
    private readonly ITravelPlanService _travelPlanService;

    public QuotesController(IQuoteService quoteService, ITravelPlanService travelPlanService)
    {
        _quoteService = quoteService;
        _travelPlanService = travelPlanService;
    }

    [HttpGet]
    public async Task<IActionResult> GetQuotes()
    {
        if (!IsValidTenant()) return TenantNotFound();

        var quotes = await _quoteService.GetByTenantAsync(TenantId);
        var response = quotes.Select(MapToResponseDto).ToList();

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetQuote(Guid id)
    {
        if (!IsValidTenant()) return TenantNotFound();

        var quote = await _quoteService.GetByIdAsync(id, TenantId);
        if (quote == null) return NotFound();

        return Ok(MapToResponseDto(quote));
    }

    [HttpPost]
    public async Task<IActionResult> CreateQuote(CreateQuoteDto dto)
    {
        if (!IsValidTenant()) return TenantNotFound();

        // Verificar que el plan existe
        var travelPlan = await _travelPlanService.GetByIdAsync(dto.TravelPlanId, TenantId);
        if (travelPlan == null) return BadRequest("Travel plan not found");

        // Calcular precio total
        var totalAmount = CalculateTotalAmount(travelPlan.BasePrice, dto.NumberOfPeople);

        var quote = new Quote
        {
            TenantId = TenantId,
            TravelPlanId = dto.TravelPlanId,
            CustomerName = dto.CustomerName,
            CustomerEmail = dto.CustomerEmail,
            CustomerPhone = dto.CustomerPhone,
            TravelDate = dto.TravelDate,
            NumberOfPeople = dto.NumberOfPeople,
            TotalAmount = totalAmount,
            Notes = dto.Notes,
            Status = QuoteStatus.Draft
        };

        var createdQuote = await _quoteService.CreateAsync(quote);
        var response = MapToResponseDto(createdQuote);

        return CreatedAtAction(nameof(GetQuote), new { id = response.Id }, response);
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateQuoteStatus(Guid id, QuoteStatus status)
    {
        if (!IsValidTenant()) return TenantNotFound();

        var quote = await _quoteService.GetByIdAsync(id, TenantId);
        if (quote == null) return NotFound();

        quote.Status = status;
        var updatedQuote = await _quoteService.UpdateAsync(quote);

        return Ok(MapToResponseDto(updatedQuote));
    }

    private decimal CalculateTotalAmount(decimal basePrice, int numberOfPeople)
    {
        // Lógica básica de pricing
        decimal total = basePrice * numberOfPeople;

        // Descuentos por grupo
        if (numberOfPeople >= 10)
            total *= 0.9m; // 10% descuento para grupos grandes
        else if (numberOfPeople >= 5)
            total *= 0.95m; // 5% descuento para grupos medianos

        return Math.Round(total, 2);
    }

    private static QuoteResponseDto MapToResponseDto(Quote quote)
    {
        return new QuoteResponseDto
        {
            Id = quote.Id,
            QuoteNumber = quote.QuoteNumber,
            CustomerName = quote.CustomerName,
            CustomerEmail = quote.CustomerEmail,
            CustomerPhone = quote.CustomerPhone,
            TravelDate = quote.TravelDate,
            NumberOfPeople = quote.NumberOfPeople,
            TotalAmount = quote.TotalAmount,
            Notes = quote.Notes,
            Status = quote.Status,
            CreatedAt = quote.CreatedAt,
            ExpiresAt = quote.ExpiresAt,
            TravelPlan = new TravelPlanResponseDto
            {
                Id = quote.TravelPlan.Id,
                Name = quote.TravelPlan.Name,
                Description = quote.TravelPlan.Description,
                BasePrice = quote.TravelPlan.BasePrice,
                DurationDays = quote.TravelPlan.DurationDays,
                Destinations = quote.TravelPlan.Destinations,
                Services = quote.TravelPlan.Services,
                Status = quote.TravelPlan.Status,
                CreatedAt = quote.TravelPlan.CreatedAt,
                UpdatedAt = quote.TravelPlan.UpdatedAt
            }
        };
    }
}