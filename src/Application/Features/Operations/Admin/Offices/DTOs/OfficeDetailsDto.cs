using Tawtheef.Application.Common.Models;

namespace Tawtheef.Application.Features.Operations.Admin.Offices.DTOs;

public sealed record OfficeDetailsDto
{
    public Guid Id { get; init; }
    public string NameAr { get; init; } = string.Empty;
    public string NameEn { get; init; } = string.Empty;
    public Guid CountryId { get; init; }
    public List<DropdownOptions> SupportedCountries { get; init; } = [];
    public string AdminEmail { get; init; } = string.Empty;
    public List<OfficeUserDto> Users { get; init; } = [];
}
