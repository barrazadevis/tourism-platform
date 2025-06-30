using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourismPlatform.Core.Entities;

public class QuoteItem
{
    public Guid Id { get; set; }
    public string ItemType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalPrice { get; set; }
    public bool IsOptional { get; set; }
    public DateTime? ServiceDate { get; set; }
    public string Notes { get; set; } = string.Empty;
    public Guid QuoteId { get; set; }

    // Navigation properties
    public Quote Quote { get; set; } = null!;
}
