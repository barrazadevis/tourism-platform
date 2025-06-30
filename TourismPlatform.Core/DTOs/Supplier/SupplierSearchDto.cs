using System;

namespace TourismPlatform.Core.DTOs.Supplier;

public class SupplierSearchDto
{
    public string? SearchTerm { get; set; }
    public string? SupplierType { get; set; }
    public string? City { get; set; }
    public bool? IsActive { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}