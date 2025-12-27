using Tawtheef.Application.Common.Models;

namespace Tawtheef.Application.Features.Operations.Admin.Offices.DTOs;

public sealed record OfficeDto
{
    public Guid Id { get; init; }
    public string NameAr { get; init; } = string.Empty;
    public string NameEn { get; init; } = string.Empty;
    public required DropdownOptions Country {get; set;
    }
    public List<DropdownOptions> SupportedCountries { get; init; } = [];
    public string AdminEmail { get; set; } = string.Empty;
}
