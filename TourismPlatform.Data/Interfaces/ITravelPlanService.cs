using TourismPlatform.Core.DTOs.Quote;
using TourismPlatform.Core.DTOs.TravelPlan;
using TourismPlatform.Core.Entities;

namespace TourismPlatform.Data.Interfaces;

public interface ITravelPlanService
{
    Task<TravelPlanResponseDto> CreateTravelPlanAsync(CreateTravelPlanDto createDto);
    Task<TravelPlanResponseDto?> GetTravelPlanByIdAsync(Guid id);
    Task<List<TravelPlanResponseDto>> GetTravelPlansAsync(TravelPlanSearchDto searchDto);
    Task<TravelPlanResponseDto?> UpdateTravelPlanAsync(Guid id, UpdateTravelPlanDto updateDto);
    Task<bool> DeleteTravelPlanAsync(Guid id);
    Task<bool> ToggleTravelPlanStatusAsync(Guid id);
    Task<List<string>> GetDestinationsAsync();
    Task<List<string>> GetPlanTypesAsync();
}