using System;
using System.ComponentModel.DataAnnotations;

namespace TourismPlatform.Core.DTOs.Booking;

public class CreatePassengerDto
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string DocumentType { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string DocumentNumber { get; set; } = string.Empty;
    
    [Required]
    public DateTime DateOfBirth { get; set; }
    
    [MaxLength(10)]
    public string Gender { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string Nationality { get; set; } = string.Empty;
    
    public bool IsMainPassenger { get; set; }
}
