using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourismPlatform.Core.Entities;

public class QuoteHotel
    {
        public Guid Id { get; set; }
        public string HotelName { get; set; } = string.Empty;
        public string HotelCategory { get; set; } = string.Empty;
        public string RoomType { get; set; } = string.Empty;
        public string PlanType { get; set; } = string.Empty;
        public int Nights { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal RoomPrice { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxesPrice { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public Guid QuoteId { get; set; }

        // Navigation properties
        public Quote Quote { get; set; } = null!;
    }
