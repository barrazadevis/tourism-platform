using System;
using Microsoft.EntityFrameworkCore;
using TourismPlatform.Core.DTOs.Company;
using TourismPlatform.Core.Entities;
using TourismPlatform.Data.Interfaces;

namespace TourismPlatform.Data.Services;

public class CompanyService : ICompanyService
{
    private readonly TourismDbContext _context;

    public CompanyService(TourismDbContext context)
    {
        _context = context;
    }

    public async Task<CompanyResponseDto> CreateCompanyAsync(CreateCompanyRequestDto request)
    {
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            PlanType = request.PlanType,
            ApplicationId = request.ApplicationId,
            SubscriptionEndsAt = request.SubscriptionEndsAt.HasValue ? DateTime.SpecifyKind(request.SubscriptionEndsAt.Value, DateTimeKind.Utc) : null,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        return await MapToResponseDto(company);
    }

    public async Task<CompanyResponseDto> GetCompanyByIdAsync(Guid id)
    {
        var company = await _context.Companies
            .Include(c => c.Application)
            .Include(c => c.Users)
            .Include(c => c.TravelPlans)
            .Include(c => c.Quotes)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (company == null)
            throw new KeyNotFoundException($"Company with ID {id} not found");

        return await MapToResponseDto(company);
    }

    public async Task<List<CompanyResponseDto>> GetAllCompaniesAsync()
    {
        var companies = await _context.Companies
            .Include(c => c.Application)
            .Include(c => c.Users)
            .Include(c => c.TravelPlans)
            .Include(c => c.Quotes)
            .ToListAsync();

        var result = new List<CompanyResponseDto>();
        foreach (var company in companies)
        {
            result.Add(await MapToResponseDto(company));
        }

        return result;
    }

    public async Task<List<CompanyResponseDto>> GetCompaniesByApplicationAsync(int applicationId)
    {
        var companies = await _context.Companies
            .Include(c => c.Application)
            .Include(c => c.Users)
            .Include(c => c.TravelPlans)
            .Include(c => c.Quotes)
            .Where(c => c.ApplicationId == applicationId)
            .ToListAsync();

        var result = new List<CompanyResponseDto>();
        foreach (var company in companies)
        {
            result.Add(await MapToResponseDto(company));
        }

        return result;
    }

    public async Task<CompanyResponseDto> UpdateCompanyAsync(Guid id, UpdateCompanyRequestDto request)
    {
        var company = await _context.Companies
            .Include(c => c.Application)
            .Include(c => c.Users)
            .Include(c => c.TravelPlans)
            .Include(c => c.Quotes)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (company == null)
            throw new KeyNotFoundException($"Company with ID {id} not found");

        if (!string.IsNullOrEmpty(request.Name))
            company.Name = request.Name;

        if (!string.IsNullOrEmpty(request.PlanType))
            company.PlanType = request.PlanType;

        if (request.SubscriptionEndsAt.HasValue)
            company.SubscriptionEndsAt = DateTime.SpecifyKind(request.SubscriptionEndsAt.Value, DateTimeKind.Utc);

        if (request.IsActive.HasValue)
            company.IsActive = request.IsActive.Value;

        await _context.SaveChangesAsync();

        return await MapToResponseDto(company);
    }

    public async Task<bool> DeleteCompanyAsync(Guid id)
    {
        var company = await _context.Companies.FirstOrDefaultAsync(c => c.Id == id);

        if (company == null)
            return false;

        _context.Companies.Remove(company);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> AssignUserToCompanyAsync(Guid companyId, int userId)
    {
        var company = await _context.Companies.FirstOrDefaultAsync(c => c.Id == companyId);
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (company == null || user == null)
            return false;

        user.CompanyId = companyId;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RemoveUserFromCompanyAsync(Guid companyId, int userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId && u.CompanyId == companyId);

        if (user == null)
            return false;

        user.CompanyId = null;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<UserResponseDto>> GetCompanyUsersAsync(Guid companyId)
    {
        var users = await _context.Users
            .Where(u => u.CompanyId == companyId)
            .ToListAsync();

        return users.Select(u => new UserResponseDto
        {
            Id = u.Id,
            Email = u.Email,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Role = u.Role,
            IsActive = u.IsActive,
            CreatedAt = u.CreatedAt
        }).ToList();
    }

    public async Task<bool> DeactivateCompanyAsync(Guid id)
    {
        var company = await _context.Companies.FirstOrDefaultAsync(c => c.Id == id);

        if (company == null)
            return false;

        company.IsActive = false;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ActivateCompanyAsync(Guid id)
    {
        var company = await _context.Companies.FirstOrDefaultAsync(c => c.Id == id);

        if (company == null)
            return false;

        company.IsActive = true;
        await _context.SaveChangesAsync();

        return true;
    }

    private async Task<CompanyResponseDto> MapToResponseDto(Company company)
    {
        return new CompanyResponseDto
        {
            Id = company.Id,
            Name = company.Name,
            PlanType = company.PlanType,
            IsActive = company.IsActive,
            CreatedAt = company.CreatedAt,
            SubscriptionEndsAt = company.SubscriptionEndsAt,
            ApplicationId = company.ApplicationId,
            ApplicationName = company.Application?.Name ?? string.Empty,
            UsersCount = company.Users?.Count ?? 0,
            TravelPlansCount = company.TravelPlans?.Count ?? 0,
            QuotesCount = company.Quotes?.Count ?? 0
        };
    }
}
