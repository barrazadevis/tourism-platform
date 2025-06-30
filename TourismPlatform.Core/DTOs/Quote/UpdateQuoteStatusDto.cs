using System;
using System.ComponentModel.DataAnnotations;
using TourismPlatform.Core.Enums;

namespace TourismPlatform.Core.DTOs.Quote;

public class UpdateQuoteStatusDto
{
    [Required]
    public QuoteStatus Status { get; set; }
    public string Notes { get; set; } = string.Empty;
}
