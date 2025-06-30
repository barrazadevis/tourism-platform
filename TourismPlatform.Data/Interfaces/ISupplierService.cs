using System;
using TourismPlatform.Core.DTOs.Supplier;

namespace TourismPlatform.Data.Interfaces;

public interface ISupplierService
{
    Task<SupplierResponseDto> CreateSupplierAsync(CreateSupplierDto createSupplierDto, Guid tenantId);
    Task<SupplierResponseDto?> GetSupplierByIdAsync(Guid id, Guid tenantId);
    Task<List<SupplierResponseDto>> GetSuppliersAsync(SupplierSearchDto searchDto, Guid tenantId);
    Task<SupplierResponseDto?> UpdateSupplierAsync(Guid id, UpdateSupplierDto updateSupplierDto, Guid tenantId);
    Task<bool> DeleteSupplierAsync(Guid id, Guid tenantId);
    Task<bool> ToggleSupplierStatusAsync(Guid id, Guid tenantId);
    Task<List<string>> GetSupplierTypesAsync(Guid tenantId);
}
