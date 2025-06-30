using System;
using System.ComponentModel.DataAnnotations;

namespace TourismPlatform.Core.DTOs.Payment;

public class CreatePaymentDto
{
    [Required]
    public Guid BookingId { get; set; }
    
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string PaymentMethod { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public string TransactionId { get; set; } = string.Empty;
    
    [MaxLength(3)]
    public string Currency { get; set; } = "COP";
    
    public string Notes { get; set; } = string.Empty;
}
