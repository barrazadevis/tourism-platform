using System;
using System.ComponentModel.DataAnnotations;

namespace TourismPlatform.Core.DTOs.Quote;

public class CreateQuoteItemDto
{
    [Required]
    public string ItemType { get; set; } = string.Empty;
    
    [Required]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
    
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal UnitPrice { get; set; }
    
    public bool IsOptional { get; set; }
    public DateTime? ServiceDate { get; set; }
    public string Notes { get; set; } = string.Empty;
}