using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TourismPlatform.Data.Interfaces;
using TourismPlatform.Core.DTOs.Quote;

namespace TourismPlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class QuotesController : ControllerBase
    {
        private readonly IQuoteService _quoteService;

        public QuotesController(IQuoteService quoteService)
        {
            _quoteService = quoteService;
        }

        [HttpGet]
        public async Task<ActionResult<List<QuoteResponseDto>>> GetQuotes(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10)
        {
            var tenantId = GetTenantId();
            var quotes = await _quoteService.GetQuotesByTenantAsync(tenantId, page, pageSize);
            return Ok(quotes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<QuoteResponseDto>> GetQuote(Guid id)
        {
            var tenantId = GetTenantId();
            var quote = await _quoteService.GetQuoteByIdAsync(id, tenantId);
            
            if (quote == null)
                return NotFound($"Quote with ID {id} not found");

            return Ok(quote);
        }

        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<List<QuoteResponseDto>>> GetQuotesByCustomer(Guid customerId)
        {
            var tenantId = GetTenantId();
            var quotes = await _quoteService.GetQuotesByCustomerAsync(customerId, tenantId);
            return Ok(quotes);
        }

        [HttpPost]
        public async Task<ActionResult<QuoteResponseDto>> CreateQuote(CreateQuoteDto createQuoteDto)
        {
            try
            {
                var tenantId = GetTenantId();
                var createdBy = GetUserName();
                
                var quote = await _quoteService.CreateQuoteAsync(createQuoteDto, tenantId, createdBy);
                
                return CreatedAtAction(
                    nameof(GetQuote), 
                    new { id = quote.Id }, 
                    quote);
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

        [HttpPut("{id}")]
        public async Task<ActionResult<QuoteResponseDto>> UpdateQuote(Guid id, CreateQuoteDto updateQuoteDto)
        {
            try
            {
                var tenantId = GetTenantId();
                var quote = await _quoteService.UpdateQuoteAsync(id, updateQuoteDto, tenantId);
                
                if (quote == null)
                    return NotFound($"Quote with ID {id} not found");

                return Ok(quote);
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

        [HttpPatch("{id}/status")]
        public async Task<ActionResult<QuoteResponseDto>> UpdateQuoteStatus(Guid id, UpdateQuoteStatusDto statusDto)
        {
            try
            {
                var tenantId = GetTenantId();
                var quote = await _quoteService.UpdateQuoteStatusAsync(id, statusDto, tenantId);
                
                if (quote == null)
                    return NotFound($"Quote with ID {id} not found");

                return Ok(quote);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteQuote(Guid id)
        {
            try
            {
                var tenantId = GetTenantId();
                var deleted = await _quoteService.DeleteQuoteAsync(id, tenantId);
                
                if (!deleted)
                    return NotFound($"Quote with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("generate-number")]
        public async Task<ActionResult<string>> GenerateQuoteNumber()
        {
            var tenantId = GetTenantId();
            var quoteNumber = await _quoteService.GenerateQuoteNumberAsync(tenantId);
            return Ok(new { quoteNumber });
        }

        [HttpGet("stats")]
        public async Task<ActionResult> GetQuoteStats()
        {
            // TODO: Implement quote statistics
            var tenantId = GetTenantId();
            
            // Placeholder response
            var stats = new
            {
                totalQuotes = 0,
                pendingQuotes = 0,
                approvedQuotes = 0,
                totalValue = 0m
            };

            return Ok(stats);
        }

        private Guid GetTenantId()
        {
            var tenantIdClaim = User.FindFirst("tenant_id")?.Value;
            if (Guid.TryParse(tenantIdClaim, out Guid tenantId))
                return tenantId;
            
            throw new UnauthorizedAccessException("Invalid tenant information");
        }

        private string GetUserName()
        {
            return User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";
        }
    }
}