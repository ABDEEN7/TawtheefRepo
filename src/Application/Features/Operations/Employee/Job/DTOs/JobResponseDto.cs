using Tawtheef.Application.Common.Models;

namespace Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

public record JobResponseDto
{
    public Guid Id { get; init; }
    public JobBasicsResponseDto Basics { get; init; } = new();
    public JobQuotasResponseDto Quotas { get; init; } = new();
    public string? Description { get; init; }
    public string? Benefits { get; init; }
    public DropdownOptions? Status { get; init; } // Now returns DropdownOptions
    public List<string> Skills { get; init; } = [];
    public List<string> Conditions { get; init; } = [];
    public List<DropdownOptions> Degrees { get; init; } = [];
}
