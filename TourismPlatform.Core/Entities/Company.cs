using System;

namespace TourismPlatform.Core.Entities;

public class Company
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PlanType { get; set; } = "Basic"; // Basic, Premium, Enterprise
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? SubscriptionEndsAt { get; set; }

    // Foreign key
    public int ApplicationId { get; set; }

    // Navigation properties
    public Application Application { get; set; } = null!;
    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<TravelPlan> TravelPlans { get; set; } = new List<TravelPlan>();
    public ICollection<Quote> Quotes { get; set; } = new List<Quote>();
}
