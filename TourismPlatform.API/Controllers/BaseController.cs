using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TourismPlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BaseController : ControllerBase
    {
        protected Guid GetCompanyId()
        {
            var companyIdClaim = User.FindFirst("company_id")?.Value;
            if (Guid.TryParse(companyIdClaim, out Guid companyId))
                return companyId;

            throw new UnauthorizedAccessException("Invalid company information");
        }

        protected string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException("Invalid user information");
        }

    }
}
