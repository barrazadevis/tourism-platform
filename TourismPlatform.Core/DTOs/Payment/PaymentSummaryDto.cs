using System;

namespace TourismPlatform.Core.DTOs.Payment;

public class PaymentSummaryDto
{
    public Guid BookingId { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal PendingAmount { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
    public List<PaymentResponseDto> Payments { get; set; } = new();
}