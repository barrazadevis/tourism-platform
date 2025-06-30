using System.ComponentModel.DataAnnotations;
using TourismPlatform.Core.Entities;
using TourismPlatform.Core.Enums;

namespace TourismPlatform.Core.DTOs.Quote;

public class CreateQuoteDto
    {
        [Required]
        public Guid CustomerId { get; set; }
        
        public Guid? TravelPlanId { get; set; }
        
        [Required]
        public DateTime DepartureDate { get; set; }
        
        [Required]
        public DateTime ReturnDate { get; set; }
        
        [Required]
        [Range(1, 50)]
        public int NumberOfAdults { get; set; }
        
        [Range(0, 20)]
        public int NumberOfChildren { get; set; }
        
        [Range(0, 10)]
        public int NumberOfInfants { get; set; }
        
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public string Currency { get; set; } = "COP";
        public int ValidityDays { get; set; } = 30;
        public string Notes { get; set; } = string.Empty;
        
        public List<CreateQuoteItemDto> Items { get; set; } = new();
        public List<CreateQuoteHotelDto> Hotels { get; set; } = new();
    }