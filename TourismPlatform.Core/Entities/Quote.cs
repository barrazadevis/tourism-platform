using System.ComponentModel.DataAnnotations.Schema;
using TourismPlatform.Core.Enums;

namespace TourismPlatform.Core.Entities;

public class Quote
{
    public Guid Id { get; set; }
    public string QuoteNumber { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public Guid? TravelPlanId { get; set; }
    public Guid? CustomDestinationId { get; set; }
    public DateTime DepartureDate { get; set; }
    public DateTime ReturnDate { get; set; }
    public int NumberOfAdults { get; set; }
    public int NumberOfChildren { get; set; }
    public int NumberOfInfants { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal SubTotal { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TaxAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal DiscountAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal PricePerPerson { get; set; }

    public QuoteStatus Status { get; set; }
    public string Currency { get; set; } = "COP";
    public DateTime ValidUntil { get; set; }
    public string Notes { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid TenantId { get; set; }

    // Navigation properties
    public Customer Customer { get; set; } = null!;
    public TravelPlan? TravelPlan { get; set; }
    public Tenant Tenant { get; set; } = null!;
    public List<QuoteItem> Items { get; set; } = new();
    public List<QuoteHotel> Hotels { get; set; } = new();
    public List<Booking> Bookings { get; set; } = new();
    public Destination? CustomDestination { get; set; }
}