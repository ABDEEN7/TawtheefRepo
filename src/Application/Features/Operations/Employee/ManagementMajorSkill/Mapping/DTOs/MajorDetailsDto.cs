using Tawtheef.Application.Common.Models;

namespace Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Mapping.DTOs;

public record MajorDetailsDto : DropdownOptions
{
    public Guid? ParentId { get; set; }
    public DropdownOptions? Parent { get; init; }

    public bool IsActive { get; init; }

    public int UsedInMappingsCount { get; set; }
}
