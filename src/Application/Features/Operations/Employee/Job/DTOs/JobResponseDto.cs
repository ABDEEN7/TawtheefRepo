using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Dtos;

public record JobResponseDto
{
    public Guid Id { get; init; }
    
    // Base entity properties
    public Guid? CreatedById { get; init; }
    public DateTimeOffset CreatedDate { get; init; }
    public Guid? UpdatedById { get; init; }
    public DateTimeOffset? UpdatedDate { get; init; }
    
    // Job properties
    public required string Title { get; init; }
    public int Vacancies { get; init; }
    public DateTimeOffset Deadline { get; init; }
    public required string Description { get; init; }
    public required string Benefits { get; init; }
    public DateTimeOffset? PublishAt { get; init; }
    
    // Navigation properties as DropdownOptions
    public DropdownOptions? RequestingDepartment { get; init; }
    public DropdownOptions? JobCategory { get; init; }
    public DropdownOptions? Gender { get; init; }
    public DropdownOptions? WorkLocation { get; init; }
    public DropdownOptions? Major { get; init; }
    public DropdownOptions? WorkType { get; init; }
    public DropdownOptions? Status { get; init; }
    
    // Complex objects
    public JobQuotasResponseDto? Quota { get; init; }
    
    // Collections
    public ICollection<DropdownOptions> Degrees { get; init; } = [];
    public ICollection<string> Conditions { get; init; } = [];
    public ICollection<string> Skills { get; init; } = [];
}
