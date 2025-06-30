using System;

namespace TourismPlatform.Core.DTOs.Quote;

public class QuoteHotelDto
{
    public string HotelName { get; set; } = string.Empty;
    public string HotelCategory { get; set; } = string.Empty;
    public string RoomType { get; set; } = string.Empty;
    public string PlanType { get; set; } = string.Empty;
    public int Nights { get; set; }
    public decimal RoomPrice { get; set; }
    public decimal TaxesPrice { get; set; }
    public decimal TotalHotelPrice { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
}