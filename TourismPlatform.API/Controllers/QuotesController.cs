using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourismPlatform.Data.Interfaces;
using TourismPlatform.Core.DTOs.Quote;

namespace TourismPlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class QuotesController : BaseController
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
            var quotes = await _quoteService.GetQuotesByCompanyAsync(page, pageSize);
            return Ok(quotes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<QuoteResponseDto>> GetQuote(Guid id)
        {
            var quote = await _quoteService.GetQuoteByIdAsync(id);

            if (quote == null)
                return NotFound($"Quote with ID {id} not found");

            return Ok(quote);
        }

        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<List<QuoteResponseDto>>> GetQuotesByCustomer(Guid customerId)
        {
            var quotes = await _quoteService.GetQuotesByCustomerAsync(customerId);
            return Ok(quotes);
        }

        [HttpPost]
        public async Task<ActionResult<QuoteResponseDto>> CreateQuote(CreateQuoteDto createQuoteDto)
        {
            try
            {
                var createdBy = GetUserId();

                var quote = await _quoteService.CreateQuoteAsync(createQuoteDto, createdBy);

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
                var quote = await _quoteService.UpdateQuoteAsync(id, updateQuoteDto);

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
                var quote = await _quoteService.UpdateQuoteStatusAsync(id, statusDto);

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
                var deleted = await _quoteService.DeleteQuoteAsync(id);

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
            var quoteNumber = await _quoteService.GenerateQuoteNumberAsync();
            return Ok(new { quoteNumber });
        }

        [HttpGet("stats")]
        public async Task<ActionResult> GetQuoteStats()
        {
            // TODO: Implement quote statistics
            var companyId = GetCompanyId();

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
    }
}