using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourismPlatform.Core.Entities;

public class BookingMetric
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public DateTime ReportDate { get; set; }
        public int TotalBookings { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalRevenue { get; set; }
        public int NewCustomers { get; set; }
        public int CancelledBookings { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal AverageBookingValue { get; set; }
        public string Period { get; set; } = string.Empty;

        // Navigation properties
        public Tenant Tenant { get; set; } = null!;
    }
