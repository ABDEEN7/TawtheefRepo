using Tawtheef.Application.Common.Models;

namespace Tawtheef.Application.Features.Operations.Admin.Universities.DTOs;

public sealed record UniversityAdminDto : DropdownOptions
{
    public string? NameAr { get; init; }
    public string? NameEn { get; init; }
    public string? DescriptionAr { get; init; }
    public string? DescriptionEn { get; init; }
    public Guid CountryId { get; init; }
    public string? CountryNameAr { get; init; }
    public string? CountryNameEn { get; init; }
    public Guid CityId { get; init; }
    public string? CityNameAr { get; init; }
    public string? CityNameEn { get; init; }
    public string? WebSite { get; init; }
    public string? Phone { get; init; }
    public string? Email { get; init; }
    public string? Code { get; init; }
    public string? LogoEn { get; init; }
    public string? LogoAr { get; init; }
    public string? OriginalName { get; init; }
    public bool IsActive { get; init; }
}
