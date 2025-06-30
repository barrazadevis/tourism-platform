using System;
using Microsoft.EntityFrameworkCore;
using TourismPlatform.Core.DTOs.Payment;
using TourismPlatform.Core.Entities;
using TourismPlatform.Core.Enums;
using TourismPlatform.Data.Interfaces;

namespace TourismPlatform.Data.Services;

public class PaymentService : IPaymentService
{
    private readonly TourismDbContext _context;

    public PaymentService(TourismDbContext context)
    {
        _context = context;
    }

    public async Task<PaymentResponseDto> CreatePaymentAsync(CreatePaymentDto createPaymentDto, Guid tenantId)
    {
        var booking = await _context.Bookings
            .Include(b => b.Quote)
            .Include(b => b.Customer)
            .Include(b => b.Payments)
            .FirstOrDefaultAsync(b => b.Id == createPaymentDto.BookingId && b.TenantId == tenantId);

        if (booking == null)
            throw new ArgumentException("Booking not found");

        if (booking.Status == BookingStatus.Cancelled)
            throw new ArgumentException("Cannot add payments to cancelled bookings");

        var totalPaid = booking.Payments.Where(p => p.PaymentStatus == PaymentStatus.Paid).Sum(p => p.Amount);
        var remainingAmount = booking.Quote.TotalAmount - totalPaid;

        if (createPaymentDto.Amount > remainingAmount)
            throw new ArgumentException($"Payment amount exceeds remaining balance of {remainingAmount:C}");

        var paymentNumber = await GeneratePaymentNumberAsync(tenantId);

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            PaymentNumber = paymentNumber,
            BookingId = createPaymentDto.BookingId,
            Amount = createPaymentDto.Amount,
            PaymentMethod = createPaymentDto.PaymentMethod,
            PaymentStatus = PaymentStatus.Paid,
            PaymentDate = DateTime.UtcNow,
            TransactionId = createPaymentDto.TransactionId,
            Currency = createPaymentDto.Currency,
            Notes = createPaymentDto.Notes,
            CreatedAt = DateTime.UtcNow,
            TenantId = tenantId
        };

        _context.Payments.Add(payment);

        // Update booking payment status
        await UpdateBookingPaymentStatus(booking);

        await _context.SaveChangesAsync();

        return await GetPaymentByIdAsync(payment.Id, tenantId) 
            ?? throw new InvalidOperationException("Failed to retrieve created payment");
    }

    public async Task<PaymentResponseDto?> GetPaymentByIdAsync(Guid id, Guid tenantId)
    {
        var payment = await _context.Payments
            .Include(p => p.Booking)
                .ThenInclude(b => b.Customer)
            .FirstOrDefaultAsync(p => p.Id == id && p.TenantId == tenantId);

        if (payment == null) return null;

        return MapToResponseDto(payment);
    }

    public async Task<List<PaymentResponseDto>> GetPaymentsByBookingAsync(Guid bookingId, Guid tenantId)
    {
        var payments = await _context.Payments
            .Include(p => p.Booking)
                .ThenInclude(b => b.Customer)
            .Where(p => p.BookingId == bookingId && p.TenantId == tenantId)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync();

        return payments.Select(MapToResponseDto).ToList();
    }

    public async Task<List<PaymentResponseDto>> GetPaymentsByTenantAsync(Guid tenantId, int page = 1, int pageSize = 10)
    {
        var payments = await _context.Payments
            .Include(p => p.Booking)
                .ThenInclude(b => b.Customer)
            .Where(p => p.TenantId == tenantId)
            .OrderByDescending(p => p.PaymentDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return payments.Select(MapToResponseDto).ToList();
    }

    public async Task<PaymentResponseDto?> UpdatePaymentStatusAsync(Guid id, UpdatePaymentStatusDto statusDto, Guid tenantId)
    {
        var payment = await _context.Payments
            .Include(p => p.Booking)
                .ThenInclude(b => b.Quote)
            .FirstOrDefaultAsync(p => p.Id == id && p.TenantId == tenantId);

        if (payment == null) return null;

        payment.PaymentStatus = statusDto.Status;
        payment.TransactionId = string.IsNullOrEmpty(statusDto.TransactionId) 
            ? payment.TransactionId 
            : statusDto.TransactionId;
        payment.Notes = string.IsNullOrEmpty(statusDto.Notes) 
            ? payment.Notes 
            : statusDto.Notes;

        // Update booking payment status
        await UpdateBookingPaymentStatus(payment.Booking);

        await _context.SaveChangesAsync();

        return await GetPaymentByIdAsync(payment.Id, tenantId);
    }

    public async Task<PaymentSummaryDto?> GetPaymentSummaryAsync(Guid bookingId, Guid tenantId)
    {
        var booking = await _context.Bookings
            .Include(b => b.Quote)
            .Include(b => b.Payments)
            .FirstOrDefaultAsync(b => b.Id == bookingId && b.TenantId == tenantId);

        if (booking == null) return null;

        var payments = await GetPaymentsByBookingAsync(bookingId, tenantId);

        return new PaymentSummaryDto
        {
            BookingId = bookingId,
            TotalAmount = booking.Quote.TotalAmount,
            TotalPaid = booking.TotalPaid,
            PendingAmount = booking.PendingAmount,
            PaymentStatus = booking.PaymentStatus.ToString(),
            Payments = payments
        };
    }

    public async Task<string> GeneratePaymentNumberAsync(Guid tenantId)
    {
        var date = DateTime.UtcNow;
        var prefix = $"PAY-{date:yyyyMM}";
        
        var lastPayment = await _context.Payments
            .Where(p => p.TenantId == tenantId && p.PaymentNumber.StartsWith(prefix))
            .OrderByDescending(p => p.PaymentNumber)
            .FirstOrDefaultAsync();

        int nextNumber = 1;
        if (lastPayment != null)
        {
            var lastNumberStr = lastPayment.PaymentNumber.Substring(prefix.Length + 1);
            if (int.TryParse(lastNumberStr, out int lastNumber))
            {
                nextNumber = lastNumber + 1;
            }
        }

        return $"{prefix}-{nextNumber:D4}";
    }

    private async Task UpdateBookingPaymentStatus(Booking booking)
    {
        var totalPaid = booking.Payments.Where(p => p.PaymentStatus == PaymentStatus.Paid).Sum(p => p.Amount);
        var totalAmount = booking.Quote?.TotalAmount ?? 0;

        booking.TotalPaid = totalPaid;
        booking.PendingAmount = totalAmount - totalPaid;

        if (totalPaid == 0)
        {
            booking.PaymentStatus = PaymentStatus.Pending;
        }
        else if (totalPaid >= totalAmount)
        {
            booking.PaymentStatus = PaymentStatus.Paid;
        }
        else
        {
            booking.PaymentStatus = PaymentStatus.Partial;
        }

        booking.UpdatedAt = DateTime.UtcNow;
    }

    private PaymentResponseDto MapToResponseDto(Payment payment)
    {
        return new PaymentResponseDto
        {
            Id = payment.Id,
            PaymentNumber = payment.PaymentNumber,
            BookingId = payment.BookingId,
            BookingNumber = payment.Booking.BookingNumber,
            CustomerName = $"{payment.Booking.Customer.FirstName} {payment.Booking.Customer.LastName}",
            Amount = payment.Amount,
            PaymentMethod = payment.PaymentMethod,
            PaymentStatus = payment.PaymentStatus.ToString(),
            PaymentDate = payment.PaymentDate,
            TransactionId = payment.TransactionId,
            Currency = payment.Currency,
            Notes = payment.Notes,
            CreatedAt = payment.CreatedAt
        };
    }
}