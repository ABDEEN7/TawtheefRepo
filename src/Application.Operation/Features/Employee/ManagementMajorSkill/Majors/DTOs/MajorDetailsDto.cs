using Tawtheef.Application.Common.Models;

namespace Application.Operation.Features.Employee.ManagementMajorSkill.Majors.DTOs;

public record MajorDetailsDto : DropdownOptions
{
    public Guid? ParentId { get; set; }
    public DropdownOptions? Parent { get; init; }

    public bool IsActive { get; init; }

    public int UsedInMappingsCount { get; set; }
}
