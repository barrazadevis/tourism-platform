using Microsoft.AspNetCore.Mvc;
using TourismPlatform.Core.DTOs;
using TourismPlatform.Core.Entities;
using TourismPlatform.Data.Interfaces;
using TourismPlatform.Data.Services;

namespace TourismPlatform.API.Controllers;

public class TravelPlansController : BaseController
{
    private readonly ITravelPlanService _travelPlanService;

    public TravelPlansController(ITravelPlanService travelPlanService)
    {
        _travelPlanService = travelPlanService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPlans()
    {
        if (!IsValidTenant()) return TenantNotFound();

        var plans = await _travelPlanService.GetByTenantAsync(TenantId);
        var response = plans.Select(MapToResponseDto).ToList();

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPlan(Guid id)
    {
        if (!IsValidTenant()) return TenantNotFound();

        var plan = await _travelPlanService.GetByIdAsync(id, TenantId);
        if (plan == null) return NotFound();

        return Ok(MapToResponseDto(plan));
    }

    [HttpPost]
    public async Task<IActionResult> CreatePlan(CreateTravelPlanDto dto)
    {
        if (!IsValidTenant()) return TenantNotFound();

        var plan = new TravelPlan
        {
            TenantId = TenantId,
            Name = dto.Name,
            Description = dto.Description,
            BasePrice = dto.BasePrice,
            DurationDays = dto.DurationDays,
            Destinations = dto.Destinations,
            Services = dto.Services,
            Status = PlanStatus.Draft
        };

        var createdPlan = await _travelPlanService.CreateAsync(plan);
        var response = MapToResponseDto(createdPlan);

        return CreatedAtAction(nameof(GetPlan), new { id = response.Id }, response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePlan(Guid id, UpdateTravelPlanDto dto)
    {
        if (!IsValidTenant()) return TenantNotFound();

        var existingPlan = await _travelPlanService.GetByIdAsync(id, TenantId);
        if (existingPlan == null) return NotFound();

        existingPlan.Name = dto.Name;
        existingPlan.Description = dto.Description;
        existingPlan.BasePrice = dto.BasePrice;
        existingPlan.DurationDays = dto.DurationDays;
        existingPlan.Destinations = dto.Destinations;
        existingPlan.Services = dto.Services;
        existingPlan.Status = dto.Status;

        var updatedPlan = await _travelPlanService.UpdateAsync(existingPlan);
        return Ok(MapToResponseDto(updatedPlan));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePlan(Guid id)
    {
        if (!IsValidTenant()) return TenantNotFound();

        var deleted = await _travelPlanService.DeleteAsync(id, TenantId);
        if (!deleted) return NotFound();

        return NoContent();
    }

    private static TravelPlanResponseDto MapToResponseDto(TravelPlan plan)
    {
        return new TravelPlanResponseDto
        {
            Id = plan.Id,
            Name = plan.Name,
            Description = plan.Description,
            BasePrice = plan.BasePrice,
            DurationDays = plan.DurationDays,
            Destinations = plan.Destinations,
            Services = plan.Services,
            Status = plan.Status,
            CreatedAt = plan.CreatedAt,
            UpdatedAt = plan.UpdatedAt
        };
    }
}
