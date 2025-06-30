using System;
using System.ComponentModel.DataAnnotations;

namespace TourismPlatform.Core.DTOs.Booking;

public class CreateBookingDto
{
    [Required]
    public Guid QuoteId { get; set; }
    
    public string SpecialRequests { get; set; } = string.Empty;
    public List<CreatePassengerDto> Passengers { get; set; } = new();
}