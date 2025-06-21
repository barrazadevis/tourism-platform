namespace TourismPlatform.Core.Entities
{
    public class Application
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public ICollection<Tenant> Tenants { get; set; } = new List<Tenant>();
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}