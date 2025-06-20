using System.ComponentModel.DataAnnotations;

namespace TourismPlatform.Core.Entities;

public class Quote
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid TravelPlanId { get; set; }
    
    [Required, MaxLength(20)]
    public string QuoteNumber { get; set; } = string.Empty;
    
    [Required, MaxLength(100)]
    public string CustomerName { get; set; } = string.Empty;
    
    [Required, MaxLength(100)]
    public string CustomerEmail { get; set; } = string.Empty;
    
    [MaxLength(20)]
    public string? CustomerPhone { get; set; }
    
    public DateTime TravelDate { get; set; }
    public int NumberOfPeople { get; set; }
    public decimal TotalAmount { get; set; }
    
    [MaxLength(500)]
    public string? Notes { get; set; }
    
    public QuoteStatus Status { get; set; } = QuoteStatus.Draft;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    
    // Navigation properties
    public virtual Tenant Tenant { get; set; } = null!;
    public virtual TravelPlan TravelPlan { get; set; } = null!;
}