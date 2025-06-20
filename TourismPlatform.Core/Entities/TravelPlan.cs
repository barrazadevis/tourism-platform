using System.ComponentModel.DataAnnotations;

namespace TourismPlatform.Core.Entities;

public class TravelPlan
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    [Required]
    public decimal BasePrice { get; set; }
    
    public int DurationDays { get; set; }
    
    [Required]
    public List<string> Destinations { get; set; } = new();
    
    public List<string> Services { get; set; } = new();
    
    public PlanStatus Status { get; set; } = PlanStatus.Draft;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public virtual Tenant Tenant { get; set; } = null!;
    public virtual ICollection<Quote> Quotes { get; set; } = new List<Quote>();
}