using System;
using System.ComponentModel.DataAnnotations.Schema;
using TourismPlatform.Core.Enums;

namespace TourismPlatform.Core.Entities;

public class Booking
    {
        public Guid Id { get; set; }
        public string BookingNumber { get; set; } = string.Empty;
        public Guid QuoteId { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public BookingStatus Status { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPaid { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal PendingAmount { get; set; }
        public DateTime BookingDate { get; set; }
        public string SpecialRequests { get; set; } = string.Empty;
        public PaymentStatus PaymentStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid TenantId { get; set; }

        // Navigation properties
        public Quote Quote { get; set; } = null!;
        public Customer Customer { get; set; } = null!;
        public Tenant Tenant { get; set; } = null!;
        public List<Passenger> Passengers { get; set; } = new();
        public List<Payment> Payments { get; set; } = new();
        public List<Document> Documents { get; set; } = new();
    }