using System;
using Microsoft.EntityFrameworkCore;
using TourismPlatform.Core.DTOs.Destination;
using TourismPlatform.Core.Entities;
using TourismPlatform.Data.Interfaces;

namespace TourismPlatform.Data.Services;

public class DestinationService : IDestinationService
{
    private readonly TourismDbContext _context;
    public DestinationService(TourismDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<DestinationDto>> GetDestinationsAsync()
    {
        return await _context.Destinations
            .Where(d => d.IsActive)
            .Select(d => new DestinationDto
            {
                Id = d.Id,
                Country = d.Country,
                City = d.City,
                Description = d.Description,
                CountryCode = d.CountryCode,
                Region = d.Region,
                IsActive = d.IsActive
            })
            .ToListAsync();
    }

    public async Task<DestinationDto?> GetDestinationByIdAsync(Guid id)
    {
        var destination = await _context.Destinations
            .FirstOrDefaultAsync(d => d.Id == id && d.IsActive);

        if (destination == null) return null;

        return new DestinationDto
        {
            Id = destination.Id,
            Country = destination.Country,
            City = destination.City,
            Description = destination.Description,
            CountryCode = destination.CountryCode,
            Region = destination.Region,
            IsActive = destination.IsActive
        };
    }

    public async Task<DestinationDto> CreateDestinationAsync(CreateDestinationDto dto)
    {
        var destination = new Destination
        {
            Country = dto.Country,
            City = dto.City,
            Description = dto.Description,
            CountryCode = dto.CountryCode,
            Region = dto.Region,
            CreatedAt = DateTime.UtcNow
        };

        _context.Destinations.Add(destination);
        await _context.SaveChangesAsync();

        return new DestinationDto
        {
            Id = destination.Id,
            Country = destination.Country,
            City = destination.City,
            Description = destination.Description,
            CountryCode = destination.CountryCode,
            Region = destination.Region,
            IsActive = destination.IsActive
        };
    }

    public async Task<DestinationDto?> UpdateDestinationAsync(Guid id, CreateDestinationDto dto)
    {
        var destination = await _context.Destinations
            .FirstOrDefaultAsync(d => d.Id == id && d.IsActive);

        if (destination == null) return null;

        destination.Country = dto.Country;
        destination.City = dto.City;
        destination.Description = dto.Description;
        destination.CountryCode = dto.CountryCode;
        destination.Region = dto.Region;
        destination.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return new DestinationDto
        {
            Id = destination.Id,
            Country = destination.Country,
            City = destination.City,
            Description = destination.Description,
            CountryCode = destination.CountryCode,
            Region = destination.Region,
            IsActive = destination.IsActive
        };
    }

    public async Task<bool> DeleteDestinationAsync(Guid id)
    {
        var destination = await _context.Destinations
            .FirstOrDefaultAsync(d => d.Id == id && d.IsActive);

        if (destination == null) return false;

        destination.IsActive = false;
        destination.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<DestinationDto>> SearchDestinationsAsync(string query)
    {
        return await _context.Destinations
            .Where(d => d.IsActive && 
                        (d.Country.Contains(query) || 
                        d.City.Contains(query) || 
                        d.Region.Contains(query)))
            .Select(d => new DestinationDto
            {
                Id = d.Id,
                Country = d.Country,
                City = d.City,
                Description = d.Description,
                CountryCode = d.CountryCode,
                Region = d.Region,
                IsActive = d.IsActive
            })
            .ToListAsync();
    }

}
