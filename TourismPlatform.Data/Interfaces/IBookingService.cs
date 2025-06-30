using System;
using TourismPlatform.Core.DTOs.Booking;

namespace TourismPlatform.Data.Interfaces;

public interface IBookingService
{
    Task<BookingResponseDto> CreateBookingFromQuoteAsync(CreateBookingDto createBookingDto, Guid tenantId);
    Task<BookingResponseDto?> GetBookingByIdAsync(Guid id, Guid tenantId);
    Task<List<BookingResponseDto>> GetBookingsByTenantAsync(Guid tenantId, int page = 1, int pageSize = 10);
    Task<List<BookingResponseDto>> GetBookingsByCustomerAsync(Guid customerId, Guid tenantId);
    Task<BookingResponseDto?> UpdateBookingStatusAsync(Guid id, UpdateBookingStatusDto statusDto, Guid tenantId);
    Task<bool> CancelBookingAsync(Guid id, string reason, Guid tenantId);
    Task<string> GenerateBookingNumberAsync(Guid tenantId);
}