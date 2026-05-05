using System.ComponentModel.DataAnnotations;

namespace TourismPlatform.Core.DTOs.Company;

public class CreateCompanyRequestDto
{
    [Required(ErrorMessage = "Company name is required")]
    [StringLength(100, ErrorMessage = "Company name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Plan type is required")]
    [RegularExpression("^(Basic|Premium|Enterprise)$", ErrorMessage = "Plan type must be Basic, Premium, or Enterprise")]
    public string PlanType { get; set; } = "Basic";

    public DateTime? SubscriptionEndsAt { get; set; }

    [Required(ErrorMessage = "Application ID is required")]
    public int ApplicationId { get; set; }
}