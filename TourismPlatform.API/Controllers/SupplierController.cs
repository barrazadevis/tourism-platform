using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourismPlatform.Core.DTOs.Supplier;
using TourismPlatform.Data.Interfaces;

namespace TourismPlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SuppliersController : ControllerBase
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
            var tenantId = GetTenantId();
            var searchDto = new SupplierSearchDto
            {
                SearchTerm = searchTerm,
                SupplierType = supplierType,
                City = city,
                IsActive = isActive,
                Page = page,
                PageSize = pageSize
            };

            var suppliers = await _supplierService.GetSuppliersAsync(searchDto, tenantId);
            return Ok(suppliers);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<SupplierResponseDto>> GetSupplier(Guid id)
        {
            var tenantId = GetTenantId();
            var supplier = await _supplierService.GetSupplierByIdAsync(id, tenantId);
            
            if (supplier == null)
                return NotFound($"Supplier with ID {id} not found");

            return Ok(supplier);
        }

        [HttpPost]
        public async Task<ActionResult<SupplierResponseDto>> CreateSupplier(CreateSupplierDto createSupplierDto)
        {
            try
            {
                var tenantId = GetTenantId();
                var supplier = await _supplierService.CreateSupplierAsync(createSupplierDto, tenantId);
                
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
                var tenantId = GetTenantId();
                var supplier = await _supplierService.UpdateSupplierAsync(id, updateSupplierDto, tenantId);
                
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
                var tenantId = GetTenantId();
                var deleted = await _supplierService.DeleteSupplierAsync(id, tenantId);
                
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
                var tenantId = GetTenantId();
                var updated = await _supplierService.ToggleSupplierStatusAsync(id, tenantId);
                
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
            var tenantId = GetTenantId();
            var types = await _supplierService.GetSupplierTypesAsync(tenantId);
            return Ok(types);
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