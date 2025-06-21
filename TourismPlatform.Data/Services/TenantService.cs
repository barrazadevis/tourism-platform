using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TourismPlatform.Core.Entities;
using TourismPlatform.Data.Interfaces;

namespace TourismPlatform.Data.Services
{
    public class TenantService : ITenantService
    {
        private readonly TourismDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TenantService(TourismDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Tenant?> GetBySubdomainAsync(string subdomain)
        {
            return await _context.Tenants
                .Include(t => t.Application)
                .FirstOrDefaultAsync(t => t.Subdomain == subdomain && t.IsActive);
        }

        public async Task<Tenant?> GetCurrentTenantAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.Items["Tenant"] is Tenant tenant)
            {
                return tenant;
            }
            return null;
        }
    }
}