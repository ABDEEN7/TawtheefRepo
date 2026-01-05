using Tawtheef.Application.Common.Models;

namespace Tawtheef.Application.Features.Operations.Employee.OrganizationStructures.DTOs;

public record SectorDto : DropdownOptions
{
    public bool IsActive { get; init; }
    public int DisplayOrder { get; init; }
}
