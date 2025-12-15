namespace Tawtheef.Application.Features.Operations.Admin.Offices.DTOs;

public sealed record CountryLookupDto
{
    public Guid Id { get; init; }
    public string NameAr { get; init; } = string.Empty;
    public string NameEn { get; init; } = string.Empty;
    public string CodeAlpha { get; init; } = string.Empty;
}
