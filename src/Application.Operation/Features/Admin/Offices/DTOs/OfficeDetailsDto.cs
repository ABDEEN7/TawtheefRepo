using Tawtheef.Application.Common.Models;

namespace Application.Operation.Features.Admin.Offices.DTOs;

public sealed record OfficeDetailsDto
{
    public Guid Id { get; init; }
    public string NameAr { get; init; } = string.Empty;
    public string NameEn { get; init; } = string.Empty;
    public Guid CountryId { get; init; }
    public List<DropdownOptions> SupportedCountries { get; init; } = [];
    public bool IsActive { get; init; }
    public string AdminNameAr { get; init; } = string.Empty;
    public string AdminNameEn { get; init; } = string.Empty;
    public string AdminEmail { get; init; } = string.Empty;
    public string? PhoneCountryCode { get; init; }
    public string? PhoneNumber { get; init; }
    public List<OfficeUserDto> Users { get; init; } = [];
}
