using Microsoft.EntityFrameworkCore;
using TourismPlatform.Core.Entities;

namespace TourismPlatform.Data.Interfaces;

public interface ITenantService
{
    Task<Tenant?> GetBySubdomainAsync(string subdomain);
    Task<Tenant?> GetByIdAsync(Guid tenantId);
    Task<Tenant> CreateAsync(Tenant tenant);
}