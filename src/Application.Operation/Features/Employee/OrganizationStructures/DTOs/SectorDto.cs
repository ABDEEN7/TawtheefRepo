using Tawtheef.Application.Common.Models;

namespace Application.Operation.Features.Employee.OrganizationStructures.DTOs;

public record SectorDto : DropdownOptions
{
    public bool IsActive { get; init; }
    public int DisplayOrder { get; init; }
}
