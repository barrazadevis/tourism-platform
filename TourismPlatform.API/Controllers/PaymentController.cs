using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourismPlatform.Core.DTOs.Payment;
using TourismPlatform.Data.Interfaces;

namespace TourismPlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet]
        public async Task<ActionResult<List<PaymentResponseDto>>> GetPayments(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var tenantId = GetTenantId();
            var payments = await _paymentService.GetPaymentsByTenantAsync(tenantId, page, pageSize);
            return Ok(payments);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PaymentResponseDto>> GetPayment(Guid id)
        {
            var tenantId = GetTenantId();
            var payment = await _paymentService.GetPaymentByIdAsync(id, tenantId);
            
            if (payment == null)
                return NotFound($"Payment with ID {id} not found");

            return Ok(payment);
        }

        [HttpGet("booking/{bookingId:guid}")]
        public async Task<ActionResult<List<PaymentResponseDto>>> GetPaymentsByBooking(Guid bookingId)
        {
            var tenantId = GetTenantId();
            var payments = await _paymentService.GetPaymentsByBookingAsync(bookingId, tenantId);
            return Ok(payments);
        }

        [HttpGet("booking/{bookingId:guid}/summary")]
        public async Task<ActionResult<PaymentSummaryDto>> GetPaymentSummary(Guid bookingId)
        {
            var tenantId = GetTenantId();
            var summary = await _paymentService.GetPaymentSummaryAsync(bookingId, tenantId);
            
            if (summary == null)
                return NotFound($"Booking with ID {bookingId} not found");

            return Ok(summary);
        }

        [HttpPost]
        public async Task<ActionResult<PaymentResponseDto>> CreatePayment(CreatePaymentDto createPaymentDto)
        {
            try
            {
                var tenantId = GetTenantId();
                var payment = await _paymentService.CreatePaymentAsync(createPaymentDto, tenantId);
                
                return CreatedAtAction(
                    nameof(GetPayment), 
                    new { id = payment.Id }, 
                    payment);
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
        public async Task<ActionResult<PaymentResponseDto>> UpdatePaymentStatus(Guid id, UpdatePaymentStatusDto statusDto)
        {
            try
            {
                var tenantId = GetTenantId();
                var payment = await _paymentService.UpdatePaymentStatusAsync(id, statusDto, tenantId);
                
                if (payment == null)
                    return NotFound($"Payment with ID {id} not found");

                return Ok(payment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("generate-number")]
        public async Task<ActionResult<string>> GeneratePaymentNumber()
        {
            var tenantId = GetTenantId();
            var paymentNumber = await _paymentService.GeneratePaymentNumberAsync(tenantId);
            return Ok(new { paymentNumber });
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