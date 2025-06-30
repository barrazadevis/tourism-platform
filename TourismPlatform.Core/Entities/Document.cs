using System;

namespace TourismPlatform.Core.Entities;

public class Document
{
    public Guid Id { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string FileExtension { get; set; } = string.Empty;
    public int FileSizeBytes { get; set; }
    public Guid? BookingId { get; set; }
    public Guid? CustomerId { get; set; }
    public DateTime UploadedAt { get; set; }
    public string UploadedBy { get; set; } = string.Empty;
    public Guid TenantId { get; set; }

    // Navigation properties
    public Booking? Booking { get; set; }
    public Customer? Customer { get; set; }
    public Tenant Tenant { get; set; } = null!;
}
