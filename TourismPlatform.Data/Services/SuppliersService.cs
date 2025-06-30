using System;
using Microsoft.EntityFrameworkCore;
using TourismPlatform.Core.DTOs.Supplier;
using TourismPlatform.Core.Entities;
using TourismPlatform.Data.Interfaces;

namespace TourismPlatform.Data.Services;

public class SuppliersService : ISupplierService
{
    private readonly TourismDbContext _context;

    public SuppliersService(TourismDbContext context)
    {
        _context = context;
    }

    public async Task<SupplierResponseDto> CreateSupplierAsync(CreateSupplierDto createSupplierDto, Guid tenantId)
    {
        var supplier = new Supplier
        {
            Id = Guid.NewGuid(),
            Name = createSupplierDto.Name,
            ContactEmail = createSupplierDto.ContactEmail,
            ContactPhone = createSupplierDto.ContactPhone,
            Address = createSupplierDto.Address,
            City = createSupplierDto.City,
            Country = createSupplierDto.Country,
            SupplierType = createSupplierDto.SupplierType,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            TenantId = tenantId
        };

        // Add services
        foreach (var serviceDto in createSupplierDto.Services)
        {
            supplier.Services.Add(new SupplierService
            {
                Id = Guid.NewGuid(),
                ServiceType = serviceDto.ServiceType,
                ServiceName = serviceDto.ServiceName,
                Description = serviceDto.Description,
                Cost = serviceDto.Cost,
                Currency = serviceDto.Currency,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });
        }

        _context.Suppliers.Add(supplier);
        await _context.SaveChangesAsync();

        return await GetSupplierByIdAsync(supplier.Id, tenantId) 
            ?? throw new InvalidOperationException("Failed to retrieve created supplier");
    }

    public async Task<SupplierResponseDto?> GetSupplierByIdAsync(Guid id, Guid tenantId)
    {
        var supplier = await _context.Suppliers
            .Include(s => s.Services)
            .FirstOrDefaultAsync(s => s.Id == id && s.TenantId == tenantId);

        if (supplier == null) return null;

        return MapToResponseDto(supplier);
    }

    public async Task<List<SupplierResponseDto>> GetSuppliersAsync(SupplierSearchDto searchDto, Guid tenantId)
    {
        var query = _context.Suppliers
            .Include(s => s.Services)
            .Where(s => s.TenantId == tenantId);

        // Apply filters
        if (!string.IsNullOrEmpty(searchDto.SearchTerm))
        {
            var searchTerm = searchDto.SearchTerm.ToLower();
            query = query.Where(s => 
                s.Name.ToLower().Contains(searchTerm) ||
                s.ContactEmail.ToLower().Contains(searchTerm) ||
                s.City.ToLower().Contains(searchTerm));
        }

        if (!string.IsNullOrEmpty(searchDto.SupplierType))
        {
            query = query.Where(s => s.SupplierType == searchDto.SupplierType);
        }

        if (!string.IsNullOrEmpty(searchDto.City))
        {
            query = query.Where(s => s.City.ToLower().Contains(searchDto.City.ToLower()));
        }

        if (searchDto.IsActive.HasValue)
        {
            query = query.Where(s => s.IsActive == searchDto.IsActive.Value);
        }

        var suppliers = await query
            .OrderBy(s => s.Name)
            .Skip((searchDto.Page - 1) * searchDto.PageSize)
            .Take(searchDto.PageSize)
            .ToListAsync();

        return suppliers.Select(MapToResponseDto).ToList();
    }

    public async Task<SupplierResponseDto?> UpdateSupplierAsync(Guid id, UpdateSupplierDto updateSupplierDto, Guid tenantId)
    {
        var supplier = await _context.Suppliers
            .Include(s => s.Services)
            .FirstOrDefaultAsync(s => s.Id == id && s.TenantId == tenantId);

        if (supplier == null) return null;

        // Update basic properties
        supplier.Name = updateSupplierDto.Name;
        supplier.ContactEmail = updateSupplierDto.ContactEmail;
        supplier.ContactPhone = updateSupplierDto.ContactPhone;
        supplier.Address = updateSupplierDto.Address;
        supplier.City = updateSupplierDto.City;
        supplier.Country = updateSupplierDto.Country;
        supplier.SupplierType = updateSupplierDto.SupplierType;

        // Remove existing services
        _context.SupplierServices.RemoveRange(supplier.Services);

        // Add updated services
        supplier.Services.Clear();
        foreach (var serviceDto in updateSupplierDto.Services)
        {
            supplier.Services.Add(new SupplierService
            {
                Id = Guid.NewGuid(),
                ServiceType = serviceDto.ServiceType,
                ServiceName = serviceDto.ServiceName,
                Description = serviceDto.Description,
                Cost = serviceDto.Cost,
                Currency = serviceDto.Currency,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync();

        return await GetSupplierByIdAsync(supplier.Id, tenantId);
    }

    public async Task<bool> DeleteSupplierAsync(Guid id, Guid tenantId)
    {
        var supplier = await _context.Suppliers
            .FirstOrDefaultAsync(s => s.Id == id && s.TenantId == tenantId);

        if (supplier == null) return false;

        _context.Suppliers.Remove(supplier);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ToggleSupplierStatusAsync(Guid id, Guid tenantId)
    {
        var supplier = await _context.Suppliers
            .FirstOrDefaultAsync(s => s.Id == id && s.TenantId == tenantId);

        if (supplier == null) return false;

        supplier.IsActive = !supplier.IsActive;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<string>> GetSupplierTypesAsync(Guid tenantId)
    {
        return await _context.Suppliers
            .Where(s => s.TenantId == tenantId)
            .Select(s => s.SupplierType)
            .Distinct()
            .OrderBy(t => t)
            .ToListAsync();
    }

    private SupplierResponseDto MapToResponseDto(Supplier supplier)
    {
        return new SupplierResponseDto
        {
            Id = supplier.Id,
            Name = supplier.Name,
            ContactEmail = supplier.ContactEmail,
            ContactPhone = supplier.ContactPhone,
            Address = supplier.Address,
            City = supplier.City,
            Country = supplier.Country,
            SupplierType = supplier.SupplierType,
            IsActive = supplier.IsActive,
            CreatedAt = supplier.CreatedAt,
            Services = supplier.Services.Select(s => new SupplierServiceDto
            {
                Id = s.Id,
                ServiceType = s.ServiceType,
                ServiceName = s.ServiceName,
                Description = s.Description,
                Cost = s.Cost,
                Currency = s.Currency,
                IsActive = s.IsActive,
                CreatedAt = s.CreatedAt
            }).ToList()
        };
    }
}
