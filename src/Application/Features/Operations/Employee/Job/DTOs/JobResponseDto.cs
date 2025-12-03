using Tawtheef.Application.Common.Models;

namespace Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

public class JobResponseDto
{
    public Guid Id { get; set; }

    // Basic Job Information
    public string Title { get; set; } = string.Empty;
    public int Vacancies { get; set; }
    public DateTimeOffset Deadline { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Benefits { get; set; } = string.Empty;
    public string? Overview { get; set; }
    public string? QualificationsDescription { get; set; }
    public DateTimeOffset? PublishAt { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset? ModifiedDate { get; set; }
    public string? CreatedBy { get; set; }
    public string? ModifiedBy { get; set; }

    // Requirements
    public int MinimumExperienceYears { get; set; }
    public int MinimumAge { get; set; }
    public int MaximumAge { get; set; }

    // Lookups
    public DropdownOptions Sector { get; set; } = default!;
    public DropdownOptions Management { get; set; } = default!;
    public DropdownOptions Department { get; set; } = default!;
    public DropdownOptions JobCategory { get; set; } = default!;
    public DropdownOptions? Gender { get; set; }
    public DropdownOptions WorkLocation { get; set; } = default!;
    public DropdownOptions Major { get; set; } = default!;
    public DropdownOptions? SubMajor { get; set; }
    public DropdownOptions WorkType { get; set; } = default!;
    public DropdownOptions Status { get; set; } = default!;

    public JobQuotaResponseDto Quota { get; set; } = default!;

    // Collections
    public List<JobDegreeResponseDto> Degrees { get; set; } = [];
    public List<JobConditionResponseDto> Conditions { get; set; } = [];
    public List<JobSkillResponseDto> Skills { get; set; } = [];
    public List<JobResponsibilityResponseDto> Responsibilities { get; set; } = [];
    public List<JobRequiredAttachmentResponseDto> RequiredAttachments { get; set; } = [];
}
