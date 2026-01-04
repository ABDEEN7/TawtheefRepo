using Tawtheef.Application.Common.Models;

namespace Tawtheef.Application.Features.Operations.Admin.Universities.DTOs;

public sealed record UniversityAdminDto : DropdownOptions
{
    public string? NameAr { get; init; }
    public string? NameEn { get; init; }
    public string? DescriptionAr { get; init; }
    public string? DescriptionEn { get; init; }
    public Guid CountryId { get; init; }
    public string? CountryName { get; set; }
    public Guid CityId { get; init; }
    public string? CityName { get; set; }
    public string? WebSite { get; init; }
    public string? Phone { get; init; }
    public string? Email { get; init; }
    public string? Code { get; init; }
    public string LogoEn { get; set; } = string.Empty;
    public string LogoAr { get; set; } = string.Empty;
    public string? OriginalName { get; init; }
    public bool IsActive { get; init; }
}
