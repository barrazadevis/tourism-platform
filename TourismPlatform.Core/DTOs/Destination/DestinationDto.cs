using System;

namespace TourismPlatform.Core.DTOs.Destination;

public class DestinationDto
{
    public Guid Id { get; set; }
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? CountryCode { get; set; }
    public string? Region { get; set; }
    public bool IsActive { get; set; }
}
