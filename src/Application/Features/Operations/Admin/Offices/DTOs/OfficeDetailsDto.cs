namespace Tawtheef.Application.Features.Operations.Admin.Offices.DTOs;

public sealed record OfficeDetailsDto
{
    public Guid Id { get; init; }
    public string NameAr { get; init; } = string.Empty;
    public string NameEn { get; init; } = string.Empty;
    public Guid CountryId { get; init; }
    public string CountryNameAr { get; init; } = string.Empty;
    public string CountryNameEn { get; init; } = string.Empty;
    public List<OfficeSupportedCountryDto> SupportedCountries { get; init; } = [];
    public string AdminEmail { get; init; } = string.Empty;
    public List<OfficeUserDto> Users { get; init; } = [];
}
