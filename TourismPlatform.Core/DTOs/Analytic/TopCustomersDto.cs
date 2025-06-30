using System;

namespace TourismPlatform.Core.DTOs.Analytic;

public class TopCustomersDto
{
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int TotalBookings { get; set; }
    public decimal TotalSpent { get; set; }
    public DateTime LastBookingDate { get; set; }
}