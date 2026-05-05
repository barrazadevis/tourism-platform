using System.Security.Claims;
using TourismPlatform.Data.Interfaces;

namespace TourismPlatform.API.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid UserId
        {
            get
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                // Si el claim por defecto no está, intenta con "sub" que es el que usa el token
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;
                }
                return Guid.TryParse(userIdClaim, out Guid userId) ? userId : Guid.Empty;
            }
        }

        public Guid CompanyId
        {
            get
            {
                var companyIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("company_id")?.Value;
                return Guid.TryParse(companyIdClaim, out Guid companyId) ? companyId : Guid.Empty;
            }
        }

        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }
}
