using TourismPlatform.Core.Entities;

namespace TourismPlatform.Data.Interfaces;

public interface ITravelPlanService
{
    Task<List<TravelPlan>> GetByTenantAsync(Guid tenantId);
    Task<TravelPlan?> GetByIdAsync(Guid id, Guid tenantId);
    Task<TravelPlan> CreateAsync(TravelPlan travelPlan);
    Task<TravelPlan> UpdateAsync(TravelPlan travelPlan);
    Task<bool> DeleteAsync(Guid id, Guid tenantId);
}