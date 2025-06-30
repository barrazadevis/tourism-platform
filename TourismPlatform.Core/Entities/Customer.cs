using System;

namespace TourismPlatform.Core.Entities;

public class Customer
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public string DocumentNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string PreferredLanguage { get; set; } = "es";
        public string Notes { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid TenantId { get; set; }

        // Navigation properties
        public Tenant Tenant { get; set; } = null!;
        public List<Quote> Quotes { get; set; } = new();
        public List<Booking> Bookings { get; set; } = new();
        public List<Passenger> Passengers { get; set; } = new();
        public List<Document> Documents { get; set; } = new();
    }
