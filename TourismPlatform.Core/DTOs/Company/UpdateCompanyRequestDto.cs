using System.ComponentModel.DataAnnotations;

namespace TourismPlatform.Core.DTOs.Company;

public class UpdateCompanyRequestDto
{
    [StringLength(100, ErrorMessage = "Company name cannot exceed 100 characters")]
    public string? Name { get; set; }

    [RegularExpression("^(Basic|Premium|Enterprise)$", ErrorMessage = "Plan type must be Basic, Premium, or Enterprise")]
    public string? PlanType { get; set; }

    public DateTime? SubscriptionEndsAt { get; set; }
    public bool? IsActive { get; set; }
}