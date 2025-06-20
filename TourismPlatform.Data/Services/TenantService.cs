using Microsoft.EntityFrameworkCore;
using TourismPlatform.Core.Entities;
using TourismPlatform.Data.Interfaces;

namespace TourismPlatform.Data.Services;

public class TenantService : ITenantService
{
    private readonly TourismDbContext _context;

    public TenantService(TourismDbContext context)
    {
        _context = context;
    }

    public async Task<Tenant?> GetBySubdomainAsync(string subdomain)
    {
        return await _context.Tenants
            .FirstOrDefaultAsync(t => t.Subdomain == subdomain && t.IsActive);
    }

    public async Task<Tenant?> GetByIdAsync(Guid tenantId)
    {
        return await _context.Tenants
            .FirstOrDefaultAsync(t => t.Id == tenantId && t.IsActive);
    }

    public async Task<Tenant> CreateAsync(Tenant tenant)
    {
        _context.Tenants.Add(tenant);
        await _context.SaveChangesAsync();
        return tenant;
    }
}