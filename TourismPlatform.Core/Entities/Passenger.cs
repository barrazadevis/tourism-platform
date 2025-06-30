using System;

namespace TourismPlatform.Core.Entities;

public class Passenger
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public string DocumentNumber { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Nationality { get; set; } = string.Empty;
        public bool IsMainPassenger { get; set; }
        public Guid BookingId { get; set; }
        public Guid CustomerId { get; set; }

        // Navigation properties
        public Booking Booking { get; set; } = null!;
        public Customer Customer { get; set; } = null!;
    }
