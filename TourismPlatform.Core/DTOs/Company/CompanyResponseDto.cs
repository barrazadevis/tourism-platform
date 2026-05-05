using System;

namespace TourismPlatform.Core.DTOs.Company;

public class CompanyResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PlanType { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? SubscriptionEndsAt { get; set; }
    public int ApplicationId { get; set; }
    public string ApplicationName { get; set; } = string.Empty;
    public int UsersCount { get; set; }
    public int TravelPlansCount { get; set; }
    public int QuotesCount { get; set; }
}
