using Tawtheef.Application.Common.Models;

namespace Tawtheef.Application.Features.Operations.Employee.OrganizationStructures.DTOs;

public record DepartmentDto : DropdownOptions
{
    public bool IsActive { get; init; }
    public int DisplayOrder { get; init; }
    public Guid ManagementId { get; init; }
    public DropdownOptions? Management { get; init; }
    public DropdownOptions? Sector { get; init; }
}
