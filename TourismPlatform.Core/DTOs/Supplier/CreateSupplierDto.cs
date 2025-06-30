using System;
using System.ComponentModel.DataAnnotations;

namespace TourismPlatform.Core.DTOs.Supplier;

public class CreateSupplierDto
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [EmailAddress]
    [MaxLength(255)]
    public string ContactEmail { get; set; } = string.Empty;
    
    [MaxLength(20)]
    public string ContactPhone { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string Address { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string City { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string Country { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string SupplierType { get; set; } = string.Empty;
    
    public List<CreateSupplierServiceDto> Services { get; set; } = new();
}
