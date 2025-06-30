using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourismPlatform.Core.DTOs.Booking;
using TourismPlatform.Data.Interfaces;

namespace TourismPlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet]
        public async Task<ActionResult<List<BookingResponseDto>>> GetBookings(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var tenantId = GetTenantId();
            var bookings = await _bookingService.GetBookingsByTenantAsync(tenantId, page, pageSize);
            return Ok(bookings);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<BookingResponseDto>> GetBooking(Guid id)
        {
            var tenantId = GetTenantId();
            var booking = await _bookingService.GetBookingByIdAsync(id, tenantId);
            
            if (booking == null)
                return NotFound($"Booking with ID {id} not found");

            return Ok(booking);
        }

        [HttpGet("customer/{customerId:guid}")]
        public async Task<ActionResult<List<BookingResponseDto>>> GetBookingsByCustomer(Guid customerId)
        {
            var tenantId = GetTenantId();
            var bookings = await _bookingService.GetBookingsByCustomerAsync(customerId, tenantId);
            return Ok(bookings);
        }

        [HttpPost("from-quote")]
        public async Task<ActionResult<BookingResponseDto>> CreateBookingFromQuote(CreateBookingDto createBookingDto)
        {
            try
            {
                var tenantId = GetTenantId();
                var booking = await _bookingService.CreateBookingFromQuoteAsync(createBookingDto, tenantId);
                
                return CreatedAtAction(
                    nameof(GetBooking), 
                    new { id = booking.Id }, 
                    booking);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPatch("{id:guid}/status")]
        public async Task<ActionResult<BookingResponseDto>> UpdateBookingStatus(Guid id, UpdateBookingStatusDto statusDto)
        {
            try
            {
                var tenantId = GetTenantId();
                var booking = await _bookingService.UpdateBookingStatusAsync(id, statusDto, tenantId);
                
                if (booking == null)
                    return NotFound($"Booking with ID {id} not found");

                return Ok(booking);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("{id:guid}/cancel")]
        public async Task<ActionResult> CancelBooking(Guid id, [FromBody] CancelBookingRequest request)
        {
            try
            {
                var tenantId = GetTenantId();
                var cancelled = await _bookingService.CancelBookingAsync(id, request.Reason, tenantId);
                
                if (!cancelled)
                    return NotFound($"Booking with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("generate-number")]
        public async Task<ActionResult<string>> GenerateBookingNumber()
        {
            var tenantId = GetTenantId();
            var bookingNumber = await _bookingService.GenerateBookingNumberAsync(tenantId);
            return Ok(new { bookingNumber });
        }

        private Guid GetTenantId()
        {
            var tenantIdClaim = User.FindFirst("tenant_id")?.Value;
            if (Guid.TryParse(tenantIdClaim, out Guid tenantId))
                return tenantId;
            
            throw new UnauthorizedAccessException("Invalid tenant information");
        }
    }

    public class CancelBookingRequest
    {
        public string Reason { get; set; } = string.Empty;
    }
}