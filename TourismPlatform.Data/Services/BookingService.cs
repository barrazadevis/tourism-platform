using System;
using Microsoft.EntityFrameworkCore;
using TourismPlatform.Core.DTOs.Booking;
using TourismPlatform.Core.Entities;
using TourismPlatform.Core.Enums;
using TourismPlatform.Data.Interfaces;

namespace TourismPlatform.Data.Services;

public class BookingService : IBookingService
{
    private readonly TourismDbContext _context;

    public BookingService(TourismDbContext context)
    {
        _context = context;
    }

    public async Task<BookingResponseDto> CreateBookingFromQuoteAsync(CreateBookingDto createBookingDto, Guid tenantId)
    {
        var quote = await _context.Quotes
            .Include(q => q.Customer)
            .FirstOrDefaultAsync(q => q.Id == createBookingDto.QuoteId && q.TenantId == tenantId);

        if (quote == null)
            throw new ArgumentException("Quote not found");

        if (quote.Status != QuoteStatus.Approved)
            throw new ArgumentException("Only approved quotes can be converted to bookings");

        var bookingNumber = await GenerateBookingNumberAsync(tenantId);

        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            BookingNumber = bookingNumber,
            QuoteId = quote.Id,
            CustomerId = quote.CustomerId,
            DepartureDate = quote.DepartureDate,
            ReturnDate = quote.ReturnDate,
            Status = BookingStatus.Pending,
            TotalPaid = 0,
            PendingAmount = quote.TotalAmount,
            BookingDate = DateTime.UtcNow,
            SpecialRequests = createBookingDto.SpecialRequests,
            PaymentStatus = PaymentStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            TenantId = tenantId
        };

        // Add passengers
        foreach (var passengerDto in createBookingDto.Passengers)
        {
            booking.Passengers.Add(new Passenger
            {
                Id = Guid.NewGuid(),
                FirstName = passengerDto.FirstName,
                LastName = passengerDto.LastName,
                DocumentType = passengerDto.DocumentType,
                DocumentNumber = passengerDto.DocumentNumber,
                DateOfBirth = passengerDto.DateOfBirth,
                Gender = passengerDto.Gender,
                Nationality = passengerDto.Nationality,
                IsMainPassenger = passengerDto.IsMainPassenger,
                CustomerId = quote.CustomerId
            });
        }

        // Update quote status
        quote.Status = QuoteStatus.Converted;
        quote.UpdatedAt = DateTime.UtcNow;

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        return await GetBookingByIdAsync(booking.Id, tenantId) 
            ?? throw new InvalidOperationException("Failed to retrieve created booking");
    }

    public async Task<BookingResponseDto?> GetBookingByIdAsync(Guid id, Guid tenantId)
    {
        var booking = await _context.Bookings
            .Include(b => b.Quote)
            .Include(b => b.Customer)
            .Include(b => b.Passengers)
            .Include(b => b.Payments)
            .FirstOrDefaultAsync(b => b.Id == id && b.TenantId == tenantId);

        if (booking == null) return null;

        return MapToResponseDto(booking);
    }

    public async Task<List<BookingResponseDto>> GetBookingsByTenantAsync(Guid tenantId, int page = 1, int pageSize = 10)
    {
        var bookings = await _context.Bookings
            .Include(b => b.Quote)
            .Include(b => b.Customer)
            .Where(b => b.TenantId == tenantId)
            .OrderByDescending(b => b.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return bookings.Select(MapToResponseDto).ToList();
    }

    public async Task<List<BookingResponseDto>> GetBookingsByCustomerAsync(Guid customerId, Guid tenantId)
    {
        var bookings = await _context.Bookings
            .Include(b => b.Quote)
            .Include(b => b.Customer)
            .Where(b => b.CustomerId == customerId && b.TenantId == tenantId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

        return bookings.Select(MapToResponseDto).ToList();
    }

    public async Task<BookingResponseDto?> UpdateBookingStatusAsync(Guid id, UpdateBookingStatusDto statusDto, Guid tenantId)
    {
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == id && b.TenantId == tenantId);

        if (booking == null) return null;

        booking.Status = statusDto.Status;
        booking.UpdatedAt = DateTime.UtcNow;

        // Update payment status based on booking status
        if (statusDto.Status == BookingStatus.Cancelled)
        {
            booking.PaymentStatus = PaymentStatus.Refunded;
        }
        else if (statusDto.Status == BookingStatus.Completed && booking.PendingAmount <= 0)
        {
            booking.PaymentStatus = PaymentStatus.Paid;
        }

        await _context.SaveChangesAsync();

        return await GetBookingByIdAsync(booking.Id, tenantId);
    }

    public async Task<bool> CancelBookingAsync(Guid id, string reason, Guid tenantId)
    {
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == id && b.TenantId == tenantId);

        if (booking == null) return false;

        booking.Status = BookingStatus.Cancelled;
        booking.SpecialRequests = $"{booking.SpecialRequests}\n\nCancellation reason: {reason}";
        booking.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<string> GenerateBookingNumberAsync(Guid tenantId)
    {
        var date = DateTime.UtcNow;
        var prefix = $"BK-{date:yyyyMM}";
        
        var lastBooking = await _context.Bookings
            .Where(b => b.TenantId == tenantId && b.BookingNumber.StartsWith(prefix))
            .OrderByDescending(b => b.BookingNumber)
            .FirstOrDefaultAsync();

        int nextNumber = 1;
        if (lastBooking != null)
        {
            var lastNumberStr = lastBooking.BookingNumber.Substring(prefix.Length + 1);
            if (int.TryParse(lastNumberStr, out int lastNumber))
            {
                nextNumber = lastNumber + 1;
            }
        }

        return $"{prefix}-{nextNumber:D4}";
    }

    private BookingResponseDto MapToResponseDto(Booking booking)
    {
        return new BookingResponseDto
        {
            Id = booking.Id,
            BookingNumber = booking.BookingNumber,
            QuoteId = booking.QuoteId,
            QuoteNumber = booking.Quote.QuoteNumber,
            CustomerId = booking.CustomerId,
            CustomerName = $"{booking.Customer.FirstName} {booking.Customer.LastName}",
            CustomerEmail = booking.Customer.Email,
            DepartureDate = booking.DepartureDate,
            ReturnDate = booking.ReturnDate,
            Status = booking.Status.ToString(),
            TotalAmount = booking.Quote.TotalAmount,
            TotalPaid = booking.TotalPaid,
            PendingAmount = booking.PendingAmount,
            BookingDate = booking.BookingDate,
            SpecialRequests = booking.SpecialRequests,
            PaymentStatus = booking.PaymentStatus.ToString(),
            CreatedAt = booking.CreatedAt,
            UpdatedAt = booking.UpdatedAt,
            Passengers = booking.Passengers.Select(p => new PassengerDto
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                FullName = $"{p.FirstName} {p.LastName}",
                DocumentType = p.DocumentType,
                DocumentNumber = p.DocumentNumber,
                DateOfBirth = p.DateOfBirth,
                Age = DateTime.Now.Year - p.DateOfBirth.Year,
                Gender = p.Gender,
                Nationality = p.Nationality,
                IsMainPassenger = p.IsMainPassenger
            }).ToList(),
            Payments = booking.Payments.Select(p => new PaymentDto
            {
                Id = p.Id,
                PaymentNumber = p.PaymentNumber,
                Amount = p.Amount,
                PaymentMethod = p.PaymentMethod,
                PaymentStatus = p.PaymentStatus.ToString(),
                PaymentDate = p.PaymentDate,
                TransactionId = p.TransactionId,
                Currency = p.Currency,
                Notes = p.Notes
            }).ToList()
        };
    }
}
