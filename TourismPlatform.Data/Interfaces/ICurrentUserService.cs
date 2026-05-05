using System;

namespace TourismPlatform.Data.Interfaces
{
    public interface ICurrentUserService
    {
        Guid UserId { get; }
        Guid CompanyId { get; }
        bool IsAuthenticated { get; }
    }
}
