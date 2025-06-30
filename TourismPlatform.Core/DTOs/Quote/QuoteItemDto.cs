using System;

namespace TourismPlatform.Core.DTOs.Quote;

public class QuoteItemDto
{
    public string ItemType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public bool IsOptional { get; set; }
    public DateTime? ServiceDate { get; set; }
    public string Notes { get; set; } = string.Empty;
}