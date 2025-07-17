using System;
using TourismPlatform.Core.DTOs.Destination;

namespace TourismPlatform.Data.Interfaces;

public interface IDestinationService
{
    Task<IEnumerable<DestinationDto>> GetDestinationsAsync();
    Task<DestinationDto?> GetDestinationByIdAsync(Guid id);
    Task<DestinationDto> CreateDestinationAsync(CreateDestinationDto dto);
    Task<DestinationDto?> UpdateDestinationAsync(Guid id, CreateDestinationDto dto);
    Task<bool> DeleteDestinationAsync(Guid id);
    Task<IEnumerable<DestinationDto>> SearchDestinationsAsync(string query);
}
