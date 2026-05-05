using System;
using TourismPlatform.Core.DTOs.Booking;

namespace TourismPlatform.Data.Interfaces;

public interface IBookingService
{
    Task<BookingResponseDto> CreateBookingFromQuoteAsync(CreateBookingDto createBookingDto);
    Task<BookingResponseDto?> GetBookingByIdAsync(Guid id);
    Task<List<BookingResponseDto>> GetBookingsByTenantAsync(int page = 1, int pageSize = 10);
    Task<List<BookingResponseDto>> GetBookingsByCustomerAsync(Guid customerId);
    Task<BookingResponseDto?> UpdateBookingStatusAsync(Guid id, UpdateBookingStatusDto statusDto);
    Task<bool> CancelBookingAsync(Guid id, string reason);
    Task<string> GenerateBookingNumberAsync();
}