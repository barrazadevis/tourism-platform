using System;
using TourismPlatform.Core.DTOs.Supplier;

namespace TourismPlatform.Data.Interfaces;

public interface ISupplierService
{
    Task<SupplierResponseDto> CreateSupplierAsync(CreateSupplierDto createSupplierDto);
    Task<SupplierResponseDto?> GetSupplierByIdAsync(Guid id);
    Task<List<SupplierResponseDto>> GetSuppliersAsync(SupplierSearchDto searchDto);
    Task<SupplierResponseDto?> UpdateSupplierAsync(Guid id, UpdateSupplierDto updateSupplierDto);
    Task<bool> DeleteSupplierAsync(Guid id);
    Task<bool> ToggleSupplierStatusAsync(Guid id);
    Task<List<string>> GetSupplierTypesAsync();
}
