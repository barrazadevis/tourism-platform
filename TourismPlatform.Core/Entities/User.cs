namespace TourismPlatform.Core.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Role { get; set; } = "User"; // User, Admin, SuperAdmin
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }

        // Foreign keys
        public Guid TenantId { get; set; }
        public Guid? CompanyId { get; set; }
        public int ApplicationId { get; set; }

        // Navigation properties
        public Tenant Tenant { get; set; } = null!;
        public Application Application { get; set; } = null!;
        public Company? Company { get; set; } = null!;

        // Computed properties
        public string FullName => $"{FirstName} {LastName}";
    }
}
