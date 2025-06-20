using Microsoft.EntityFrameworkCore;
using TourismPlatform.Core.Entities;
using TourismPlatform.Data.Interfaces;

namespace TourismPlatform.Data.Services;
public class TravelPlanService : ITravelPlanService
{
    private readonly TourismDbContext _context;

    public TravelPlanService(TourismDbContext context)
    {
        _context = context;
    }

    public async Task<List<TravelPlan>> GetByTenantAsync(Guid tenantId)
    {
        return await _context.TravelPlans
            .Where(tp => tp.TenantId == tenantId)
            .OrderByDescending(tp => tp.CreatedAt)
            .ToListAsync();
    }

    public async Task<TravelPlan?> GetByIdAsync(Guid id, Guid tenantId)
    {
        return await _context.TravelPlans
            .FirstOrDefaultAsync(tp => tp.Id == id && tp.TenantId == tenantId);
    }

    public async Task<TravelPlan> CreateAsync(TravelPlan travelPlan)
    {
        travelPlan.Id = Guid.NewGuid();
        travelPlan.CreatedAt = DateTime.UtcNow;

        _context.TravelPlans.Add(travelPlan);
        await _context.SaveChangesAsync();
        return travelPlan;
    }

    public async Task<TravelPlan> UpdateAsync(TravelPlan travelPlan)
    {
        travelPlan.UpdatedAt = DateTime.UtcNow;
        _context.TravelPlans.Update(travelPlan);
        await _context.SaveChangesAsync();
        return travelPlan;
    }

    public async Task<bool> DeleteAsync(Guid id, Guid tenantId)
    {
        var travelPlan = await GetByIdAsync(id, tenantId);
        if (travelPlan == null) return false;

        _context.TravelPlans.Remove(travelPlan);
        await _context.SaveChangesAsync();
        return true;
    }
}