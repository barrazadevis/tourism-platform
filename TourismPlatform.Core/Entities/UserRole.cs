namespace TourismPlatform.Core.Entities;

public enum UserRole
{
    User = 0,
    Admin = 1,
    SuperAdmin = 2
}

public enum PlanStatus
{
    Draft = 0,
    Active = 1,
    Inactive = 2,
    Archived = 3
}

public enum QuoteStatus
{
    Draft = 0,
    Sent = 1,
    Viewed = 2,
    Accepted = 3,
    Rejected = 4,
    Expired = 5
}