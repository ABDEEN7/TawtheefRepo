using Tawtheef.Application.Common.Models;

namespace Tawtheef.Application.Features.Operations.Admin.Universities.DTOs;

public sealed record UniversityAdminDto : DropdownOptions
{
    public string? NameAr { get; set; }
    public string? NameEn { get; set; }
    public string? DescriptionAr { get; set; }
    public string? DescriptionEn { get; set; }
    public Guid CountryId { get; set; }
    public string? CountryName { get; set; }
    public Guid CityId { get; set; }
    public string? CityName { get; set; }
    public string? WebSite { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Code { get; set; }
    public string? LogoEn { get; set; }
    public string? LogoAr { get; set; }
    public string? OriginalName { get; set; }
    public bool IsActive { get; set; }
}
