using System;
using System.ComponentModel.DataAnnotations;

namespace TourismPlatform.Core.Entities;

public class Destination
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Country { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string City { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Description { get; set; }

    [StringLength(10)]
    public string? CountryCode { get; set; } // ISO 3166-1 alpha-2 (US, CO, etc.)

    [StringLength(50)]
    public string? Region { get; set; } // Región/Estado/Provincia

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<TravelPlan> TravelPlans { get; set; } = new List<TravelPlan>();
    public ICollection<Quote> Quotes { get; set; } = new List<Quote>();
}
