namespace Tawtheef.Application.Features.Operations.Admin.Offices.DTOs;

public sealed record OfficeSupportedCountryDto
{
    public Guid CountryId { get; init; }
    public string NameAr { get; init; } = string.Empty;
    public string NameEn { get; init; } = string.Empty;
}
