using System;
using System.ComponentModel.DataAnnotations;

namespace TourismPlatform.Core.DTOs.Supplier;

public class CreateSupplierServiceDto
{
    [Required]
    [MaxLength(100)]
    public string ServiceType { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(200)]
    public string ServiceName { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Cost { get; set; }
    
    [MaxLength(3)]
    public string Currency { get; set; } = "COP";
}