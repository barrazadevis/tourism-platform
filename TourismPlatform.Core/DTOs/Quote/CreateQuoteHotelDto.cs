using System;
using System.ComponentModel.DataAnnotations;

namespace TourismPlatform.Core.DTOs.Quote;

public class CreateQuoteHotelDto
{
    [Required]
    public string HotelName { get; set; } = string.Empty;
    
    public string HotelCategory { get; set; } = string.Empty;
    public string RoomType { get; set; } = string.Empty;
    public string PlanType { get; set; } = string.Empty;
    
    [Required]
    [Range(1, 365)]
    public int Nights { get; set; }
    
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal RoomPrice { get; set; }
    
    public decimal TaxesPrice { get; set; }
    
    [Required]
    public DateTime CheckInDate { get; set; }
    
    [Required]
    public DateTime CheckOutDate { get; set; }
}