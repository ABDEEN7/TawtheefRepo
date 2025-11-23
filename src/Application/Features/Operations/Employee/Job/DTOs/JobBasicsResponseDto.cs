using Tawtheef.Application.Common.Models;

namespace Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

public record JobBasicsResponseDto
{
    public string Title { get; init; } = string.Empty;
    public int Vacancies { get; init; }
    public DateTimeOffset Deadline { get; init; }
    public DropdownOptions? Department { get; init; } // Now returns DropdownOptions
    public DropdownOptions? JobCategory { get; init; } // Now returns DropdownOptions
    public DropdownOptions? Major { get; init; } // Now returns DropdownOptions
    public DropdownOptions? WorkType { get; init; } // Now returns DropdownOptions
    public DropdownOptions? Gender { get; init; } // Now returns DropdownOptions
    public DropdownOptions? TargetEntity { get; init; } // Now returns DropdownOptions
}
