using System.ComponentModel.DataAnnotations;

namespace TourismPlatform.Core.DTOs.Customer;

public class CreateCustomerDto
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;
    
    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string DocumentType { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string DocumentNumber { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string Address { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string City { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string Country { get; set; } = string.Empty;
    
    public DateTime? DateOfBirth { get; set; }
    
    [MaxLength(5)]
    public string PreferredLanguage { get; set; } = "es";
    
    public string Notes { get; set; } = string.Empty;
}