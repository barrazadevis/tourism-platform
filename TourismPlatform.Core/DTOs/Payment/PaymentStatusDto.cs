using System;
using System.ComponentModel.DataAnnotations;
using TourismPlatform.Core.Enums;

namespace TourismPlatform.Core.DTOs.Payment;

public class UpdatePaymentStatusDto
{
    [Required]
    public PaymentStatus Status { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}