using Microsoft.EntityFrameworkCore;
using TourismPlatform.Core.DTOs.Quote;
using TourismPlatform.Core.Entities;
using TourismPlatform.Core.Enums;
using TourismPlatform.Data.Common;
using TourismPlatform.Data.Interfaces;

namespace TourismPlatform.Data.Services;
public class QuoteService : IQuoteService
{
    private readonly TourismDbContext _context;

    public QuoteService(TourismDbContext context)
    {
        _context = context;
    }

    public async Task<QuoteResponseDto> CreateQuoteAsync(CreateQuoteDto createQuoteDto, string createdBy)
    {
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == createQuoteDto.CustomerId);

        if (customer == null)
            throw new ArgumentException("Customer not found");

        var quoteNumber = await GenerateQuoteNumberAsync();
        DateTimeUtils.EnsureUtcDateTimes(createQuoteDto);
        var quote = new Quote
        {
            QuoteNumber = quoteNumber,
            CustomerId = createQuoteDto.CustomerId,
            TravelPlanId = createQuoteDto.TravelPlanId,
            DepartureDate = createQuoteDto.DepartureDate,
            ReturnDate = createQuoteDto.ReturnDate,
            NumberOfAdults = createQuoteDto.NumberOfAdults,
            NumberOfChildren = createQuoteDto.NumberOfChildren,
            NumberOfInfants = createQuoteDto.NumberOfInfants,
            TaxAmount = createQuoteDto.TaxAmount,
            DiscountAmount = createQuoteDto.DiscountAmount,
            Currency = createQuoteDto.Currency,
            Status = QuoteStatus.Draft,
            ValidUntil = DateTime.UtcNow.AddDays(createQuoteDto.ValidityDays),
            Notes = createQuoteDto.Notes,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };

        // Add items
        foreach (var itemDto in createQuoteDto.Items)
        {
            var totalPrice = itemDto.UnitPrice * itemDto.Quantity;
            quote.Items.Add(new QuoteItem
            {
                ItemType = itemDto.ItemType,
                Description = itemDto.Description,
                Quantity = itemDto.Quantity,
                UnitPrice = itemDto.UnitPrice,
                TotalPrice = totalPrice,
                IsOptional = itemDto.IsOptional,
                ServiceDate = itemDto.ServiceDate,
                Notes = itemDto.Notes
            });
        }

        // Add hotels
        foreach (var hotelDto in createQuoteDto.Hotels)
        {
            quote.Hotels.Add(new QuoteHotel
            {
                HotelName = hotelDto.HotelName,
                HotelCategory = hotelDto.HotelCategory,
                RoomType = hotelDto.RoomType,
                PlanType = hotelDto.PlanType,
                Nights = hotelDto.Nights,
                RoomPrice = hotelDto.RoomPrice,
                TaxesPrice = hotelDto.TaxesPrice,
                CheckInDate = hotelDto.CheckInDate,
                CheckOutDate = hotelDto.CheckOutDate
            });
        }

        // Calculate totals
        CalculateQuoteTotals(quote);

        _context.Quotes.Add(quote);
        await _context.SaveChangesAsync();

        return await GetQuoteByIdAsync(quote.Id) ?? throw new InvalidOperationException("Failed to retrieve created quote");
    }

    public async Task<QuoteResponseDto?> GetQuoteByIdAsync(Guid id)
    {
        var quote = await _context.Quotes
            .Include(q => q.Customer)
            .Include(q => q.TravelPlan)
            .Include(q => q.Items)
            .Include(q => q.Hotels)
            .FirstOrDefaultAsync(q => q.Id == id);

        if (quote == null) return null;

        return MapToResponseDto(quote);
    }

    public async Task<List<QuoteResponseDto>> GetQuotesByCompanyAsync(int page = 1, int pageSize = 10)
    {
        var quotes = await _context.Quotes
            .Include(q => q.Customer)
            .Include(q => q.TravelPlan)
            .OrderByDescending(q => q.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return quotes.Select(MapToResponseDto).ToList();
    }

    public async Task<List<QuoteResponseDto>> GetQuotesByCustomerAsync(Guid customerId)
    {
        var quotes = await _context.Quotes
            .Include(q => q.Customer)
            .Include(q => q.TravelPlan)
            .Where(q => q.CustomerId == customerId)
            .OrderByDescending(q => q.CreatedAt)
            .ToListAsync();

        return quotes.Select(MapToResponseDto).ToList();
    }

    public async Task<QuoteResponseDto?> UpdateQuoteAsync(Guid id, CreateQuoteDto updateQuoteDto)
    {
        var quote = await _context.Quotes
            .Include(q => q.Items)
            .Include(q => q.Hotels)
            .FirstOrDefaultAsync(q => q.Id == id);

        if (quote == null) return null;

        // Update basic properties
        quote.DepartureDate = updateQuoteDto.DepartureDate;
        quote.ReturnDate = updateQuoteDto.ReturnDate;
        quote.NumberOfAdults = updateQuoteDto.NumberOfAdults;
        quote.NumberOfChildren = updateQuoteDto.NumberOfChildren;
        quote.NumberOfInfants = updateQuoteDto.NumberOfInfants;
        quote.TaxAmount = updateQuoteDto.TaxAmount;
        quote.DiscountAmount = updateQuoteDto.DiscountAmount;
        quote.Notes = updateQuoteDto.Notes;
        quote.UpdatedAt = DateTime.UtcNow;

        // Remove existing items and hotels
        _context.QuoteItems.RemoveRange(quote.Items);
        _context.QuoteHotels.RemoveRange(quote.Hotels);

        // Add updated items
        quote.Items.Clear();
        foreach (var itemDto in updateQuoteDto.Items)
        {
            var totalPrice = itemDto.UnitPrice * itemDto.Quantity;
            quote.Items.Add(new QuoteItem
            {
                ItemType = itemDto.ItemType,
                Description = itemDto.Description,
                Quantity = itemDto.Quantity,
                UnitPrice = itemDto.UnitPrice,
                TotalPrice = totalPrice,
                IsOptional = itemDto.IsOptional,
                ServiceDate = itemDto.ServiceDate,
                Notes = itemDto.Notes
            });
        }

        // Add updated hotels
        quote.Hotels.Clear();
        foreach (var hotelDto in updateQuoteDto.Hotels)
        {
            quote.Hotels.Add(new QuoteHotel
            {
                HotelName = hotelDto.HotelName,
                HotelCategory = hotelDto.HotelCategory,
                RoomType = hotelDto.RoomType,
                PlanType = hotelDto.PlanType,
                Nights = hotelDto.Nights,
                RoomPrice = hotelDto.RoomPrice,
                TaxesPrice = hotelDto.TaxesPrice,
                CheckInDate = hotelDto.CheckInDate,
                CheckOutDate = hotelDto.CheckOutDate
            });
        }

        // Recalculate totals
        CalculateQuoteTotals(quote);

        await _context.SaveChangesAsync();

        return await GetQuoteByIdAsync(quote.Id);
    }

    public async Task<QuoteResponseDto?> UpdateQuoteStatusAsync(Guid id, UpdateQuoteStatusDto statusDto)
    {
        var quote = await _context.Quotes
            .FirstOrDefaultAsync(q => q.Id == id);

        if (quote == null) return null;

        quote.Status = statusDto.Status;
        quote.Notes = string.IsNullOrEmpty(statusDto.Notes) ? quote.Notes : statusDto.Notes;
        quote.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return await GetQuoteByIdAsync(quote.Id);
    }

    public async Task<bool> DeleteQuoteAsync(Guid id)
    {
        var quote = await _context.Quotes
            .FirstOrDefaultAsync(q => q.Id == id);

        if (quote == null) return false;

        _context.Quotes.Remove(quote);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<string> GenerateQuoteNumberAsync()
    {
        var date = DateTime.UtcNow;
        var prefix = $"QT-{date:yyyyMM}";
        
        var lastQuote = await _context.Quotes
            .Where(q => q.QuoteNumber.StartsWith(prefix))
            .OrderByDescending(q => q.QuoteNumber)
            .FirstOrDefaultAsync();

        int nextNumber = 1;
        if (lastQuote != null)
        {
            var lastNumberStr = lastQuote.QuoteNumber.Substring(prefix.Length + 1);
            if (int.TryParse(lastNumberStr, out int lastNumber))
            {
                nextNumber = lastNumber + 1;
            }
        }

        return $"{prefix}-{nextNumber:D4}";
    }

    private void CalculateQuoteTotals(Quote quote)
    {
        // Calculate subtotal from items and hotels
        var itemsTotal = quote.Items.Sum(i => i.TotalPrice);
        var hotelsTotal = quote.Hotels.Sum(h => h.RoomPrice + h.TaxesPrice);
        
        quote.SubTotal = itemsTotal + hotelsTotal;
        quote.TotalAmount = quote.SubTotal + quote.TaxAmount - quote.DiscountAmount;
        
        var totalPassengers = quote.NumberOfAdults + quote.NumberOfChildren + quote.NumberOfInfants;
        quote.PricePerPerson = totalPassengers > 0 ? quote.TotalAmount / totalPassengers : 0;
    }

    private QuoteResponseDto MapToResponseDto(Quote quote)
    {
        return new QuoteResponseDto
        {
            Id = quote.Id,
            QuoteNumber = quote.QuoteNumber,
            CustomerId = quote.CustomerId,
            CustomerName = $"{quote.Customer.FirstName} {quote.Customer.LastName}",
            CustomerEmail = quote.Customer.Email,
            TravelPlanName = quote.TravelPlan?.Name ?? "",
            DepartureDate = quote.DepartureDate,
            ReturnDate = quote.ReturnDate,
            TotalPassengers = quote.NumberOfAdults + quote.NumberOfChildren + quote.NumberOfInfants,
            NumberOfAdults = quote.NumberOfAdults,
            NumberOfChildren = quote.NumberOfChildren,
            NumberOfInfants = quote.NumberOfInfants,
            SubTotal = quote.SubTotal,
            TaxAmount = quote.TaxAmount,
            DiscountAmount = quote.DiscountAmount,
            TotalAmount = quote.TotalAmount,
            PricePerPerson = quote.PricePerPerson,
            Status = quote.Status.ToString(),
            Currency = quote.Currency,
            ValidUntil = quote.ValidUntil,
            Notes = quote.Notes,
            CreatedAt = quote.CreatedAt,
            Items = quote.Items.Select(i => new QuoteItemDto
            {
                ItemType = i.ItemType,
                Description = i.Description,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                TotalPrice = i.TotalPrice,
                IsOptional = i.IsOptional,
                ServiceDate = i.ServiceDate,
                Notes = i.Notes
            }).ToList(),
            Hotels = quote.Hotels.Select(h => new QuoteHotelDto
            {
                HotelName = h.HotelName,
                HotelCategory = h.HotelCategory,
                RoomType = h.RoomType,
                PlanType = h.PlanType,
                Nights = h.Nights,
                RoomPrice = h.RoomPrice,
                TaxesPrice = h.TaxesPrice,
                TotalHotelPrice = h.RoomPrice + h.TaxesPrice,
                CheckInDate = h.CheckInDate,
                CheckOutDate = h.CheckOutDate
            }).ToList()
        };
    }
}