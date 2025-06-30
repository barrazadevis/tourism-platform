using System;

namespace TourismPlatform.Core.DTOs.Document;

public class DocumentSearchDto
{
    public string? DocumentType { get; set; }
    public Guid? BookingId { get; set; }
    public Guid? CustomerId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}