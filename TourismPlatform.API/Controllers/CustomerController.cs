using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourismPlatform.Core.DTOs.Customer;
using TourismPlatform.Data.Interfaces;

namespace TourismPlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CustomersController : BaseController
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<ActionResult<List<CustomerResponseDto>>> GetCustomers(
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? documentNumber = null,
            [FromQuery] string? email = null,
            [FromQuery] string? phone = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var searchDto = new CustomerSearchDto
            {
                SearchTerm = searchTerm,
                DocumentNumber = documentNumber,
                Email = email,
                Phone = phone,
                Page = page,
                PageSize = pageSize
            };

            var customers = await _customerService.GetCustomersAsync(searchDto);
            return Ok(customers);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<CustomerResponseDto>> GetCustomer(Guid id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);

            if (customer == null)
                return NotFound($"Customer with ID {id} not found");

            return Ok(customer);
        }

        [HttpGet("by-email/{email}")]
        public async Task<ActionResult<CustomerResponseDto>> GetCustomerByEmail(string email)
        {
            var customer = await _customerService.GetCustomerByEmailAsync(email);

            if (customer == null)
                return NotFound($"Customer with email {email} not found");

            return Ok(customer);
        }

        [HttpGet("by-document/{documentNumber}")]
        public async Task<ActionResult<CustomerResponseDto>> GetCustomerByDocument(string documentNumber)
        {
            var customer = await _customerService.GetCustomerByDocumentAsync(documentNumber);

            if (customer == null)
                return NotFound($"Customer with document {documentNumber} not found");

            return Ok(customer);
        }

        [HttpPost]
        public async Task<ActionResult<CustomerResponseDto>> CreateCustomer(CreateCustomerDto createCustomerDto)
        {
            try
            {
                var customer = await _customerService.CreateCustomerAsync(createCustomerDto);

                return CreatedAtAction(
                    nameof(GetCustomer),
                    new { id = customer.Id },
                    customer);
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

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<CustomerResponseDto>> UpdateCustomer(Guid id, UpdateCustomerDto updateCustomerDto)
        {
            try
            {
                var customer = await _customerService.UpdateCustomerAsync(id, updateCustomerDto);

                if (customer == null)
                    return NotFound($"Customer with ID {id} not found");

                return Ok(customer);
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

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> DeleteCustomer(Guid id)
        {
            try
            {
                var deleted = await _customerService.DeleteCustomerAsync(id);

                if (!deleted)
                    return NotFound($"Customer with ID {id} not found");

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

        [HttpGet("exists")]
        public async Task<ActionResult<bool>> CheckCustomerExists(
            [FromQuery] string email,
            [FromQuery] string documentNumber,
            [FromQuery] Guid? excludeId = null)
        {
            var exists = await _customerService.ExistsAsync(email, documentNumber, excludeId);
            return Ok(new { exists });
        }

        [HttpGet("stats")]
        public async Task<ActionResult> GetCustomerStats()
        {
            // TODO: Implement customer statistics
            var stats = new
            {
                totalCustomers = 0,
                newThisMonth = 0,
                topSpenders = new List<object>(),
                averageSpent = 0m
            };

            return Ok(stats);
        }
    }
}