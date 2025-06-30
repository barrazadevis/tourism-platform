using System;
using TourismPlatform.Core.DTOs.Payment;

namespace TourismPlatform.Data.Interfaces;

public interface IPaymentService
{
    Task<PaymentResponseDto> CreatePaymentAsync(CreatePaymentDto createPaymentDto, Guid tenantId);
    Task<PaymentResponseDto?> GetPaymentByIdAsync(Guid id, Guid tenantId);
    Task<List<PaymentResponseDto>> GetPaymentsByBookingAsync(Guid bookingId, Guid tenantId);
    Task<List<PaymentResponseDto>> GetPaymentsByTenantAsync(Guid tenantId, int page = 1, int pageSize = 10);
    Task<PaymentResponseDto?> UpdatePaymentStatusAsync(Guid id, UpdatePaymentStatusDto statusDto, Guid tenantId);
    Task<PaymentSummaryDto?> GetPaymentSummaryAsync(Guid bookingId, Guid tenantId);
    Task<string> GeneratePaymentNumberAsync(Guid tenantId);
}
