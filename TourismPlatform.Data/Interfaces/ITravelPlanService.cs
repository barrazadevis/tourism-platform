using TourismPlatform.Core.DTOs.Quote;
using TourismPlatform.Core.DTOs.TravelPlan;
using TourismPlatform.Core.Entities;

namespace TourismPlatform.Data.Interfaces;

public interface ITravelPlanService
{
    Task<TravelPlanResponseDto> CreateTravelPlanAsync(CreateTravelPlanDto createDto, Guid tenantId);
    Task<TravelPlanResponseDto?> GetTravelPlanByIdAsync(Guid id, Guid tenantId);
    Task<List<TravelPlanResponseDto>> GetTravelPlansAsync(TravelPlanSearchDto searchDto, Guid tenantId);
    Task<TravelPlanResponseDto?> UpdateTravelPlanAsync(Guid id, UpdateTravelPlanDto updateDto, Guid tenantId);
    Task<bool> DeleteTravelPlanAsync(Guid id, Guid tenantId);
    Task<bool> ToggleTravelPlanStatusAsync(Guid id, Guid tenantId);
    Task<List<string>> GetDestinationsAsync(Guid tenantId);
    Task<List<string>> GetPlanTypesAsync(Guid tenantId);
}