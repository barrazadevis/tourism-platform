using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourismPlatform.Core.DTOs.TravelPlan;
using TourismPlatform.Data.Interfaces;

namespace TourismPlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TravelPlansController : ControllerBase
    {
        private readonly ITravelPlanService _travelPlanService;

        public TravelPlansController(ITravelPlanService travelPlanService)
        {
            _travelPlanService = travelPlanService;
        }

        [HttpGet]
        public async Task<ActionResult<List<TravelPlanResponseDto>>> GetTravelPlans(
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? destination = null,
            [FromQuery] string? planType = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] int? minDuration = null,
            [FromQuery] int? maxDuration = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var tenantId = GetTenantId();
            var searchDto = new TravelPlanSearchDto
            {
                SearchTerm = searchTerm,
                Destination = destination,
                PlanType = planType,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                MinDuration = minDuration,
                MaxDuration = maxDuration,
                IsActive = isActive,
                Page = page,
                PageSize = pageSize
            };

            var travelPlans = await _travelPlanService.GetTravelPlansAsync(searchDto, tenantId);
            return Ok(travelPlans);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<TravelPlanResponseDto>> GetTravelPlan(Guid id)
        {
            var tenantId = GetTenantId();
            var travelPlan = await _travelPlanService.GetTravelPlanByIdAsync(id, tenantId);
            
            if (travelPlan == null)
                return NotFound($"Travel plan with ID {id} not found");

            return Ok(travelPlan);
        }

        [HttpPost]
        public async Task<ActionResult<TravelPlanResponseDto>> CreateTravelPlan(CreateTravelPlanDto createDto)
        {
            try
            {
                var tenantId = GetTenantId();
                var travelPlan = await _travelPlanService.CreateTravelPlanAsync(createDto, tenantId);
                
                return CreatedAtAction(
                    nameof(GetTravelPlan), 
                    new { id = travelPlan.Id }, 
                    travelPlan);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<TravelPlanResponseDto>> UpdateTravelPlan(Guid id, UpdateTravelPlanDto updateDto)
        {
            try
            {
                var tenantId = GetTenantId();
                var travelPlan = await _travelPlanService.UpdateTravelPlanAsync(id, updateDto, tenantId);
                
                if (travelPlan == null)
                    return NotFound($"Travel plan with ID {id} not found");

                return Ok(travelPlan);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> DeleteTravelPlan(Guid id)
        {
            try
            {
                var tenantId = GetTenantId();
                var deleted = await _travelPlanService.DeleteTravelPlanAsync(id, tenantId);
                
                if (!deleted)
                    return NotFound($"Travel plan with ID {id} not found");

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPatch("{id:guid}/toggle-status")]
        public async Task<ActionResult> ToggleTravelPlanStatus(Guid id)
        {
            try
            {
                var tenantId = GetTenantId();
                var updated = await _travelPlanService.ToggleTravelPlanStatusAsync(id, tenantId);
                
                if (!updated)
                    return NotFound($"Travel plan with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("destinations")]
        public async Task<ActionResult<List<string>>> GetDestinations()
        {
            var tenantId = GetTenantId();
            var destinations = await _travelPlanService.GetDestinationsAsync(tenantId);
            return Ok(destinations);
        }

        [HttpGet("plan-types")]
        public async Task<ActionResult<List<string>>> GetPlanTypes()
        {
            var tenantId = GetTenantId();
            var planTypes = await _travelPlanService.GetPlanTypesAsync(tenantId);
            return Ok(planTypes);
        }

        private Guid GetTenantId()
        {
            var tenantIdClaim = User.FindFirst("tenant_id")?.Value;
            if (Guid.TryParse(tenantIdClaim, out Guid tenantId))
                return tenantId;
            
            throw new UnauthorizedAccessException("Invalid tenant information");
        }
    }
}