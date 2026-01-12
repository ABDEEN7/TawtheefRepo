using Tawtheef.Application.Common.Models;

namespace Application.Operation.Features.Employee.OrganizationStructures.DTOs;

public record ManagementDto : DropdownOptions
{
    public bool IsActive { get; init; }
    public int DisplayOrder { get; init; }
    public Guid SectorId { get; init; }
    public DropdownOptions? Sector { get; init; }
}
