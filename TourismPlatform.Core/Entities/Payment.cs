using System;
using System.ComponentModel.DataAnnotations.Schema;
using TourismPlatform.Core.Enums;

namespace TourismPlatform.Core.Entities;

public class Payment
{
    public Guid Id { get; set; }
    public string PaymentNumber { get; set; } = string.Empty;
    public Guid BookingId { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public PaymentStatus PaymentStatus { get; set; }
    public DateTime PaymentDate { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public string Currency { get; set; } = "COP";
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public Guid TenantId { get; set; }

    // Navigation properties
    public Booking Booking { get; set; } = null!;
    public Tenant Tenant { get; set; } = null!;
}