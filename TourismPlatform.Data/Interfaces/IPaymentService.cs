using System;
using TourismPlatform.Core.DTOs.Payment;

namespace TourismPlatform.Data.Interfaces;

public interface IPaymentService
{
    Task<PaymentResponseDto> CreatePaymentAsync(CreatePaymentDto createPaymentDto);
    Task<PaymentResponseDto?> GetPaymentByIdAsync(Guid id);
    Task<List<PaymentResponseDto>> GetPaymentsByBookingAsync(Guid bookingId);
    Task<List<PaymentResponseDto>> GetPaymentsByCompanyAsync(int page = 1, int pageSize = 10);
    Task<PaymentResponseDto?> UpdatePaymentStatusAsync(Guid id, UpdatePaymentStatusDto statusDto);
    Task<PaymentSummaryDto?> GetPaymentSummaryAsync(Guid bookingId);
    Task<string> GeneratePaymentNumberAsync();
}
