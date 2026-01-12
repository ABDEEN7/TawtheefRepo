using Tawtheef.Application.Common.Models;

namespace Application.Operation.Features.Employee.OrganizationStructures.DTOs;

public record DepartmentDto : DropdownOptions
{
    public bool IsActive { get; init; }
    public int DisplayOrder { get; init; }
    public Guid ManagementId { get; init; }
    public DropdownOptions? Management { get; init; }
    public DropdownOptions? Sector { get; init; }
}
