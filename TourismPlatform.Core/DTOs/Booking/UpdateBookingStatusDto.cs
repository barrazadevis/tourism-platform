using System;
using System.ComponentModel.DataAnnotations;
using TourismPlatform.Core.Enums;

namespace TourismPlatform.Core.DTOs.Booking;

public class UpdateBookingStatusDto
{
    [Required]
    public BookingStatus Status { get; set; }
    public string Notes { get; set; } = string.Empty;
}