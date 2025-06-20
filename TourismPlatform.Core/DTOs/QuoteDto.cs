using System.ComponentModel.DataAnnotations;
using TourismPlatform.Core.Entities;

namespace TourismPlatform.Core.DTOs;

public class CreateQuoteDto
{
    [Required]
    public Guid TravelPlanId { get; set; }
    
    [Required, MaxLength(100)]
    public string CustomerName { get; set; } = string.Empty;
    
    [Required, EmailAddress, MaxLength(100)]
    public string CustomerEmail { get; set; } = string.Empty;
    
    [Phone, MaxLength(20)]
    public string? CustomerPhone { get; set; }
    
    [Required]
    public DateTime TravelDate { get; set; }
    
    [Required, Range(1, 50)]
    public int NumberOfPeople { get; set; }
    
    [MaxLength(500)]
    public string? Notes { get; set; }
}

public class QuoteResponseDto
{
    public Guid Id { get; set; }
    public string QuoteNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string? CustomerPhone { get; set; }
    public DateTime TravelDate { get; set; }
    public int NumberOfPeople { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Notes { get; set; }
    public QuoteStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    
    // Travel Plan info
    public TravelPlanResponseDto TravelPlan { get; set; } = null!;
}