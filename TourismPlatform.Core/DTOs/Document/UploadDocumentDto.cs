using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
namespace TourismPlatform.Core.DTOs.Document;

public class UploadDocumentDto
{
    [Required]
    public IFormFile File { get; set; } = null!;
    
    [Required]
    [MaxLength(100)]
    public string DocumentType { get; set; } = string.Empty;
    
    public Guid? BookingId { get; set; }
    public Guid? CustomerId { get; set; }
}
