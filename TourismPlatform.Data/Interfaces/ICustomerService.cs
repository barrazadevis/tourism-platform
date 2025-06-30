using System;
using TourismPlatform.Core.DTOs.Customer;

namespace TourismPlatform.Data.Interfaces;

public interface ICustomerService
{
    Task<CustomerResponseDto> CreateCustomerAsync(CreateCustomerDto createCustomerDto, Guid tenantId);
    Task<CustomerResponseDto?> GetCustomerByIdAsync(Guid id, Guid tenantId);
    Task<List<CustomerResponseDto>> GetCustomersAsync(CustomerSearchDto searchDto, Guid tenantId);
    Task<CustomerResponseDto?> GetCustomerByEmailAsync(string email, Guid tenantId);
    Task<CustomerResponseDto?> GetCustomerByDocumentAsync(string documentNumber, Guid tenantId);
    Task<CustomerResponseDto?> UpdateCustomerAsync(Guid id, UpdateCustomerDto updateCustomerDto, Guid tenantId);
    Task<bool> DeleteCustomerAsync(Guid id, Guid tenantId);
    Task<bool> ExistsAsync(string email, string documentNumber, Guid tenantId, Guid? excludeId = null);
}
