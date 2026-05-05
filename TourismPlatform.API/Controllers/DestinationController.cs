using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourismPlatform.Core.DTOs.Destination;
using TourismPlatform.Data.Interfaces;

namespace TourismPlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DestinationController : BaseController
    {
        private readonly IDestinationService _destinationService;

        public DestinationController(IDestinationService destinationService)
        {
            _destinationService = destinationService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DestinationDto>>> GetDestinations()
        {
            var destinations = await _destinationService.GetDestinationsAsync();
            return Ok(destinations);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DestinationDto>> GetDestination(Guid id)
        {
            var destination = await _destinationService.GetDestinationByIdAsync(id);
            if (destination == null)
            {
                return NotFound();
            }
            return Ok(destination);
        }

        [HttpPost]
        public async Task<ActionResult<DestinationDto>> CreateDestination(CreateDestinationDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var destination = await _destinationService.CreateDestinationAsync(dto);
            return CreatedAtAction(nameof(GetDestination), new { id = destination.Id }, destination);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<DestinationDto>> UpdateDestination(Guid id, CreateDestinationDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var destination = await _destinationService.UpdateDestinationAsync(id, dto);
            if (destination == null)
            {
                return NotFound();
            }
            return Ok(destination);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteDestination(Guid id)
        {
            var result = await _destinationService.DeleteDestinationAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<DestinationDto>>> SearchDestinations([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Query parameter is required");
            }

            var destinations = await _destinationService.SearchDestinationsAsync(query);
            return Ok(destinations);
        }
    }
}
