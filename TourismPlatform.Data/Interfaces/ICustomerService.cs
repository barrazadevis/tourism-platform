using System;
using TourismPlatform.Core.DTOs.Customer;

namespace TourismPlatform.Data.Interfaces;

public interface ICustomerService
{
    Task<CustomerResponseDto> CreateCustomerAsync(CreateCustomerDto createCustomerDto);
    Task<CustomerResponseDto?> GetCustomerByIdAsync(Guid id);
    Task<List<CustomerResponseDto>> GetCustomersAsync(CustomerSearchDto searchDto);
    Task<CustomerResponseDto?> GetCustomerByEmailAsync(string email);
    Task<CustomerResponseDto?> GetCustomerByDocumentAsync(string documentNumber);
    Task<CustomerResponseDto?> UpdateCustomerAsync(Guid id, UpdateCustomerDto updateCustomerDto);
    Task<bool> DeleteCustomerAsync(Guid id);
    Task<bool> ExistsAsync(string email, string documentNumber, Guid? excludeId = null);
}
