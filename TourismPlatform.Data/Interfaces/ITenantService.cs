using TourismPlatform.Core.Entities;

namespace TourismPlatform.Data.Interfaces
{
    public interface ITenantService
    {
        Task<Tenant?> GetBySubdomainAsync(string subdomain);
        Task<Tenant?> GetCurrentTenantAsync();
    }
}