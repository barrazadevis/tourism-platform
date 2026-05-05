using Microsoft.EntityFrameworkCore;
using TourismPlatform.Core.DTOs.TravelPlan;
using TourismPlatform.Core.Entities;
using TourismPlatform.Data.Interfaces;

namespace TourismPlatform.Data.Services;

public class TravelPlanService : ITravelPlanService
    {
        private readonly TourismDbContext _context;

        public TravelPlanService(TourismDbContext context)
        {
            _context = context;
        }

        public async Task<TravelPlanResponseDto> CreateTravelPlanAsync(CreateTravelPlanDto createDto)
        {
            var travelPlan = new TravelPlan
            {
                Id = Guid.NewGuid(),
                Name = createDto.Name,
                Description = createDto.Description,
                DestinationId = createDto.DestinationId,
                DurationDays = createDto.DurationDays,
                BasePrice = createDto.BasePrice,
                Inclusions = string.Join(", ", createDto.Inclusions),
                Exclusions = string.Join(", ", createDto.Exclusions),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            // Add services
            foreach (var serviceDto in createDto.Services)
            {
                travelPlan.Services.Add(new PlanService
                {
                    Id = Guid.NewGuid(),
                    ServiceType = serviceDto.ServiceType,
                    Name = serviceDto.Name,
                    Description = serviceDto.Description,
                    Price = serviceDto.Price,
                    IsIncluded = serviceDto.IsIncluded,
                    IsOptional = serviceDto.IsOptional
                });
            }

            _context.TravelPlans.Add(travelPlan);
            await _context.SaveChangesAsync();

            return await GetTravelPlanByIdAsync(travelPlan.Id) 
                ?? throw new InvalidOperationException("Failed to retrieve created travel plan");
        }

        public async Task<TravelPlanResponseDto?> GetTravelPlanByIdAsync(Guid id)
        {
            var travelPlan = await _context.TravelPlans
                .Include(tp => tp.Services)
                .Include(tp => tp.Quotes)
                .FirstOrDefaultAsync(tp => tp.Id == id);

            if (travelPlan == null) return null;

            return MapToResponseDto(travelPlan);
        }

        public async Task<List<TravelPlanResponseDto>> GetTravelPlansAsync(TravelPlanSearchDto searchDto)
        {
            var query = _context.TravelPlans
                .Include(tp => tp.Services)
                .Include(tp => tp.Quotes)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(searchDto.SearchTerm))
            {
                var searchTerm = searchDto.SearchTerm.ToLower();
                query = query.Where(tp => 
                    tp.Name.ToLower().Contains(searchTerm) ||
                    tp.Description.ToLower().Contains(searchTerm) ||
                    tp.DestinationId.ToString().Contains(searchTerm)
                );
            }

            if (!string.IsNullOrEmpty(searchDto.Destination))
            {
                query = query.Where(tp => tp.DestinationId.ToString().Contains(searchDto.Destination.ToLower()));
            }

            if (!string.IsNullOrEmpty(searchDto.PlanType))
            {
                query = query.Where(tp => tp.PlanType == searchDto.PlanType);
            }

            if (searchDto.MinPrice.HasValue)
            {
                query = query.Where(tp => tp.BasePrice >= searchDto.MinPrice);
            }

            if (searchDto.MaxPrice.HasValue)
            {
                query = query.Where(tp => tp.BasePrice <= searchDto.MaxPrice);
            }

            if (searchDto.MinDuration.HasValue)
            {
                query = query.Where(tp => tp.DurationDays >= searchDto.MinDuration);
            }

            if (searchDto.MaxDuration.HasValue)
            {
                query = query.Where(tp => tp.DurationDays <= searchDto.MaxDuration);
            }

            if (searchDto.IsActive.HasValue)
            {
                query = query.Where(tp => tp.IsActive == searchDto.IsActive);
            }

            var travelPlans = await query
                .OrderBy(tp => tp.Name)
                .Skip((searchDto.Page - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize)
                .ToListAsync();

            return travelPlans.Select(MapToResponseDto).ToList();
        }

        public async Task<TravelPlanResponseDto?> UpdateTravelPlanAsync(Guid id, UpdateTravelPlanDto updateDto)
        {
            var travelPlan = await _context.TravelPlans
                .Include(tp => tp.Services)
                .FirstOrDefaultAsync(tp => tp.Id == id);

            if (travelPlan == null) return null;

            // Update basic properties
            travelPlan.Name = updateDto.Name;
            travelPlan.Description = updateDto.Description;
            travelPlan.DestinationId = updateDto.DestinationId;
            travelPlan.DurationDays = updateDto.DurationDays;
            travelPlan.BasePrice = updateDto.BasePrice;
            travelPlan.Inclusions = string.Join(", ", updateDto.Inclusions);
            travelPlan.Exclusions = string.Join(", ", updateDto.Exclusions);
            travelPlan.UpdatedAt = DateTime.UtcNow;

            // Remove existing services
            _context.PlanServices.RemoveRange(travelPlan.Services);

            // Add updated services
            travelPlan.Services.Clear();
            foreach (var serviceDto in updateDto.Services)
            {
                travelPlan.Services.Add(new PlanService
                {
                    Id = Guid.NewGuid(),
                    ServiceType = serviceDto.ServiceType,
                    Name = serviceDto.Name,
                    Description = serviceDto.Description,
                    Price = serviceDto.Price,
                    IsIncluded = serviceDto.IsIncluded,
                    IsOptional = serviceDto.IsOptional
                });
            }

            await _context.SaveChangesAsync();

            return await GetTravelPlanByIdAsync(travelPlan.Id);
        }

        public async Task<bool> DeleteTravelPlanAsync(Guid id)
        {
            var travelPlan = await _context.TravelPlans
                .Include(tp => tp.Quotes)
                .FirstOrDefaultAsync(tp => tp.Id == id);

            if (travelPlan == null) return false;

            // Check if travel plan has quotes
            if (travelPlan.Quotes.Any())
            {
                throw new InvalidOperationException("Cannot delete travel plan with existing quotes");
            }

            _context.TravelPlans.Remove(travelPlan);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ToggleTravelPlanStatusAsync(Guid id)
        {
            var travelPlan = await _context.TravelPlans
                .FirstOrDefaultAsync(tp => tp.Id == id);

            if (travelPlan == null) return false;

            travelPlan.IsActive = !travelPlan.IsActive;
            travelPlan.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<string>> GetDestinationsAsync()
        {
            return await _context.TravelPlans
                .Where(tp => tp.IsActive)
                .Select(tp => tp.DestinationId.ToString())
                .Distinct()
                .OrderBy(d => d)
                .ToListAsync();
        }

        public async Task<List<string>> GetPlanTypesAsync()
        {
            return await _context.TravelPlans
                .Select(tp => tp.PlanType)
                .Distinct()
                .OrderBy(pt => pt)
                .ToListAsync();
        }

        private TravelPlanResponseDto MapToResponseDto(TravelPlan travelPlan)
        {
            var totalPrice = travelPlan.BasePrice + travelPlan.Services.Where(s => s.IsIncluded).Sum(s => s.Price);

            return new TravelPlanResponseDto
            {
                Id = travelPlan.Id,
                Name = travelPlan.Name,
                Description = travelPlan.Description,
                DestinationId = travelPlan.DestinationId,
                DurationDays = travelPlan.DurationDays,
                BasePrice = travelPlan.BasePrice,
                PlanType = travelPlan.PlanType,
                Inclusions = travelPlan.Inclusions,
                Exclusions = travelPlan.Exclusions,
                IsActive = travelPlan.IsActive,
                CreatedAt = travelPlan.CreatedAt,
                UpdatedAt = travelPlan.UpdatedAt,
                Services = travelPlan.Services.Select(s => new PlanServiceResponseDto
                {
                    Id = s.Id,
                    ServiceType = s.ServiceType,
                    Name = s.Name,
                    Description = s.Description,
                    Price = s.Price,
                    IsIncluded = s.IsIncluded,
                    IsOptional = s.IsOptional
                }).ToList(),
                TotalQuotes = travelPlan.Quotes.Count,
                TotalPrice = totalPrice
            };
        }
    }