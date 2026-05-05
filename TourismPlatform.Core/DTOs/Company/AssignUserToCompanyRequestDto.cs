using System.ComponentModel.DataAnnotations;

namespace TourismPlatform.Core.DTOs.Company;

public class AssignUserToCompanyRequestDto
{
    [Required(ErrorMessage = "User ID is required")]
    public int UserId { get; set; }
}