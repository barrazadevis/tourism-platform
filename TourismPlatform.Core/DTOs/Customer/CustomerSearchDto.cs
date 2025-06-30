using System;

namespace TourismPlatform.Core.DTOs.Customer;

public class CustomerSearchDto
{
    public string? SearchTerm { get; set; }
    public string? DocumentNumber { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}