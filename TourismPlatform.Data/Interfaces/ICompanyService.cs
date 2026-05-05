using System;
using TourismPlatform.Core.DTOs.Company;

namespace TourismPlatform.Data.Interfaces;

public interface ICompanyService
{
    Task<CompanyResponseDto> CreateCompanyAsync(CreateCompanyRequestDto request);
    Task<CompanyResponseDto> GetCompanyByIdAsync(Guid id);
    Task<List<CompanyResponseDto>> GetAllCompaniesAsync();
    Task<List<CompanyResponseDto>> GetCompaniesByApplicationAsync(int applicationId);
    Task<CompanyResponseDto> UpdateCompanyAsync(Guid id, UpdateCompanyRequestDto request);
    Task<bool> DeleteCompanyAsync(Guid id);
    Task<bool> AssignUserToCompanyAsync(Guid companyId, int userId);
    Task<bool> RemoveUserFromCompanyAsync(Guid companyId, int userId);
    Task<List<UserResponseDto>> GetCompanyUsersAsync(Guid companyId);
    Task<bool> DeactivateCompanyAsync(Guid id);
    Task<bool> ActivateCompanyAsync(Guid id);
}
