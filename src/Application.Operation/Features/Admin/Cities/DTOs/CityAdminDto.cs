using Tawtheef.Application.Common.Models;

namespace Application.Operation.Features.Admin.Cities.DTOs;

public sealed record CityAdminDto : DropdownOptions
{
    public bool IsActive { get; init; }
}
