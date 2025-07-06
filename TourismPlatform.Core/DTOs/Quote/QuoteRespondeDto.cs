using System;

namespace TourismPlatform.Core.DTOs.Quote;

public class QuoteResponseDto
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public string QuoteNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string TravelPlanName { get; set; } = string.Empty;
    public DateTime DepartureDate { get; set; }
    public DateTime ReturnDate { get; set; }
    public int TotalPassengers { get; set; }
    public int NumberOfAdults { get; set; }
    public int NumberOfChildren { get; set; }
    public int NumberOfInfants { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PricePerPerson { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public DateTime ValidUntil { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    
    public List<QuoteItemDto> Items { get; set; } = new();
    public List<QuoteHotelDto> Hotels { get; set; } = new();
}
