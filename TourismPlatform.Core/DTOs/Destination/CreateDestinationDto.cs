using System;
using System.ComponentModel.DataAnnotations;

namespace TourismPlatform.Core.DTOs.Destination;

public class CreateDestinationDto
{
    [Required]
    [StringLength(100)]
    public string Country { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string City { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Description { get; set; }

    [StringLength(10)]
    public string? CountryCode { get; set; }

    [StringLength(50)]
    public string? Region { get; set; }
}
