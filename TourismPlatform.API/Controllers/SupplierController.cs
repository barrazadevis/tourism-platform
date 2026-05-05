using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourismPlatform.Core.DTOs.Supplier;
using TourismPlatform.Data.Interfaces;

namespace TourismPlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SuppliersController : BaseController
    {
        private readonly ISupplierService _supplierService;

        public SuppliersController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        [HttpGet]
        public async Task<ActionResult<List<SupplierResponseDto>>> GetSuppliers(
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? supplierType = null,
            [FromQuery] string? city = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var searchDto = new SupplierSearchDto
            {
                SearchTerm = searchTerm,
                SupplierType = supplierType,
                City = city,
                IsActive = isActive,
                Page = page,
                PageSize = pageSize
            };

            var suppliers = await _supplierService.GetSuppliersAsync(searchDto);
            return Ok(suppliers);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<SupplierResponseDto>> GetSupplier(Guid id)
        {
            var supplier = await _supplierService.GetSupplierByIdAsync(id);

            if (supplier == null)
                return NotFound($"Supplier with ID {id} not found");

            return Ok(supplier);
        }

        [HttpPost]
        public async Task<ActionResult<SupplierResponseDto>> CreateSupplier(CreateSupplierDto createSupplierDto)
        {
            try
            {
                var supplier = await _supplierService.CreateSupplierAsync(createSupplierDto);

                return CreatedAtAction(
                    nameof(GetSupplier), 
                    new { id = supplier.Id }, 
                    supplier);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<SupplierResponseDto>> UpdateSupplier(Guid id, UpdateSupplierDto updateSupplierDto)
        {
            try
            {
                var supplier = await _supplierService.UpdateSupplierAsync(id, updateSupplierDto);

                if (supplier == null)
                    return NotFound($"Supplier with ID {id} not found");

                return Ok(supplier);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> DeleteSupplier(Guid id)
        {
            try
            {
                var deleted = await _supplierService.DeleteSupplierAsync(id);

                if (!deleted)
                    return NotFound($"Supplier with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPatch("{id:guid}/toggle-status")]
        public async Task<ActionResult> ToggleSupplierStatus(Guid id)
        {
            try
            {
                var updated = await _supplierService.ToggleSupplierStatusAsync(id);

                if (!updated)
                    return NotFound($"Supplier with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("types")]
        public async Task<ActionResult<List<string>>> GetSupplierTypes()
        {
            var types = await _supplierService.GetSupplierTypesAsync();
            return Ok(types);
        }
    }
}