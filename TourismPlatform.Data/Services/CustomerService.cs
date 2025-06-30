using System;
using Microsoft.EntityFrameworkCore;
using TourismPlatform.Core.DTOs.Customer;
using TourismPlatform.Core.Entities;
using TourismPlatform.Core.Enums;
using TourismPlatform.Data.Interfaces;

namespace TourismPlatform.Data.Services;

public class CustomerService : ICustomerService
{
    private readonly TourismDbContext _context;

    public CustomerService(TourismDbContext context)
    {
        _context = context;
    }

    public async Task<CustomerResponseDto> CreateCustomerAsync(CreateCustomerDto createCustomerDto, Guid tenantId)
    {
        // Check if customer already exists
        if (await ExistsAsync(createCustomerDto.Email, createCustomerDto.DocumentNumber, tenantId))
        {
            throw new ArgumentException("Customer with this email or document number already exists");
        }

        var customer = new Customer
        {
            FirstName = createCustomerDto.FirstName,
            LastName = createCustomerDto.LastName,
            Email = createCustomerDto.Email,
            Phone = createCustomerDto.Phone,
            DocumentType = createCustomerDto.DocumentType,
            DocumentNumber = createCustomerDto.DocumentNumber,
            Address = createCustomerDto.Address,
            City = createCustomerDto.City,
            Country = createCustomerDto.Country,
            DateOfBirth = createCustomerDto.DateOfBirth,
            PreferredLanguage = createCustomerDto.PreferredLanguage,
            Notes = createCustomerDto.Notes,
            CreatedAt = DateTime.UtcNow,
            TenantId = tenantId
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        return await GetCustomerByIdAsync(customer.Id, tenantId) 
            ?? throw new InvalidOperationException("Failed to retrieve created customer");
    }

    public async Task<CustomerResponseDto?> GetCustomerByIdAsync(Guid id, Guid tenantId)
    {
        var customer = await _context.Customers
            .Include(c => c.Quotes)
            .Include(c => c.Bookings)
            .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == tenantId);

        if (customer == null) return null;

        return MapToResponseDto(customer);
    }

    public async Task<List<CustomerResponseDto>> GetCustomersAsync(CustomerSearchDto searchDto, Guid tenantId)
    {
        var query = _context.Customers
            .Include(c => c.Quotes)
            .Include(c => c.Bookings)
            .Where(c => c.TenantId == tenantId);

        // Apply search filters
        if (!string.IsNullOrEmpty(searchDto.SearchTerm))
        {
            var searchTerm = searchDto.SearchTerm.ToLower();
            query = query.Where(c => 
                c.FirstName.ToLower().Contains(searchTerm) ||
                c.LastName.ToLower().Contains(searchTerm) ||
                c.Email.ToLower().Contains(searchTerm) ||
                c.Phone.Contains(searchTerm));
        }

        if (!string.IsNullOrEmpty(searchDto.DocumentNumber))
        {
            query = query.Where(c => c.DocumentNumber.Contains(searchDto.DocumentNumber));
        }

        if (!string.IsNullOrEmpty(searchDto.Email))
        {
            query = query.Where(c => c.Email.ToLower().Contains(searchDto.Email.ToLower()));
        }

        if (!string.IsNullOrEmpty(searchDto.Phone))
        {
            query = query.Where(c => c.Phone.Contains(searchDto.Phone));
        }

        var customers = await query
            .OrderBy(c => c.FirstName)
            .ThenBy(c => c.LastName)
            .Skip((searchDto.Page - 1) * searchDto.PageSize)
            .Take(searchDto.PageSize)
            .ToListAsync();

        return customers.Select(MapToResponseDto).ToList();
    }

    public async Task<CustomerResponseDto?> GetCustomerByEmailAsync(string email, Guid tenantId)
    {
        var customer = await _context.Customers
            .Include(c => c.Quotes)
            .Include(c => c.Bookings)
            .FirstOrDefaultAsync(c => c.Email.ToLower() == email.ToLower() && c.TenantId == tenantId);

        if (customer == null) return null;

        return MapToResponseDto(customer);
    }

    public async Task<CustomerResponseDto?> GetCustomerByDocumentAsync(string documentNumber, Guid tenantId)
    {
        var customer = await _context.Customers
            .Include(c => c.Quotes)
            .Include(c => c.Bookings)
            .FirstOrDefaultAsync(c => c.DocumentNumber == documentNumber && c.TenantId == tenantId);

        if (customer == null) return null;

        return MapToResponseDto(customer);
    }

    public async Task<CustomerResponseDto?> UpdateCustomerAsync(Guid id, UpdateCustomerDto updateCustomerDto, Guid tenantId)
    {
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == tenantId);

        if (customer == null) return null;

        // Check if email or document exists (excluding current customer)
        if (await ExistsAsync(updateCustomerDto.Email, updateCustomerDto.DocumentNumber, tenantId, id))
        {
            throw new ArgumentException("Customer with this email or document number already exists");
        }

        // Update properties
        customer.FirstName = updateCustomerDto.FirstName;
        customer.LastName = updateCustomerDto.LastName;
        customer.Email = updateCustomerDto.Email;
        customer.Phone = updateCustomerDto.Phone;
        customer.DocumentType = updateCustomerDto.DocumentType;
        customer.DocumentNumber = updateCustomerDto.DocumentNumber;
        customer.Address = updateCustomerDto.Address;
        customer.City = updateCustomerDto.City;
        customer.Country = updateCustomerDto.Country;
        customer.DateOfBirth = updateCustomerDto.DateOfBirth;
        customer.PreferredLanguage = updateCustomerDto.PreferredLanguage;
        customer.Notes = updateCustomerDto.Notes;
        customer.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return await GetCustomerByIdAsync(customer.Id, tenantId);
    }

    public async Task<bool> DeleteCustomerAsync(Guid id, Guid tenantId)
    {
        var customer = await _context.Customers
            .Include(c => c.Quotes)
            .Include(c => c.Bookings)
            .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == tenantId);

        if (customer == null) return false;

        // Check if customer has quotes or bookings
        if (customer.Quotes.Any() || customer.Bookings.Any())
        {
            throw new InvalidOperationException("Cannot delete customer with existing quotes or bookings");
        }

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ExistsAsync(string email, string documentNumber, Guid tenantId, Guid? excludeId = null)
    {
        var query = _context.Customers.Where(c => c.TenantId == tenantId);

        if (excludeId.HasValue)
        {
            query = query.Where(c => c.Id != excludeId.Value);
        }

        return await query.AnyAsync(c => 
            c.Email.ToLower() == email.ToLower() || 
            c.DocumentNumber == documentNumber);
    }

    private CustomerResponseDto MapToResponseDto(Customer customer)
    {
        var age = customer.DateOfBirth.HasValue 
            ? DateTime.Now.Year - customer.DateOfBirth.Value.Year 
            : (int?)null;

        if (age.HasValue && customer.DateOfBirth.Value.AddYears(age.Value) > DateTime.Now)
        {
            age--;
        }

        var totalSpent = customer.Bookings
            .Where(b => b.Status == BookingStatus.Completed)
            .Sum(b => b.TotalPaid);

        return new CustomerResponseDto
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            FullName = $"{customer.FirstName} {customer.LastName}",
            Email = customer.Email,
            Phone = customer.Phone,
            DocumentType = customer.DocumentType,
            DocumentNumber = customer.DocumentNumber,
            Address = customer.Address,
            City = customer.City,
            Country = customer.Country,
            DateOfBirth = customer.DateOfBirth,
            Age = age,
            PreferredLanguage = customer.PreferredLanguage,
            Notes = customer.Notes,
            CreatedAt = customer.CreatedAt,
            UpdatedAt = customer.UpdatedAt,
            TotalQuotes = customer.Quotes.Count,
            TotalBookings = customer.Bookings.Count,
            TotalSpent = totalSpent
        };
    }
}