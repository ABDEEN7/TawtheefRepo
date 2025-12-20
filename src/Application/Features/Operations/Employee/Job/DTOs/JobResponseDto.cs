using Tawtheef.Application.Common.Models;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

public class JobResponseDto
{
    public Guid Id { get; set; }
    public string? TitleAr { get; set; } = string.Empty;
    public string? TitleEn { get; set; } = string.Empty;
    public int NumberOfVacancies { get; set; }
    public DateTime ClosingDate { get; set; }
    public string? BenefitsAr { get; set; } = string.Empty;
    public string? BenefitsEn { get; set; } = string.Empty;
    public string? OverViewAr { get; set; } = string.Empty;
    public string? OverViewEn { get; set; } = string.Empty;
    public string? QualificationDescriptionAr { get; set; } = string.Empty;
    public string? QualificationDescriptionEn { get; set; } = string.Empty;
    public DateTimeOffset? PublishAt { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset? ModifiedDate { get; set; }
    public User? CreatedBy { get; set; }
    public User? ModifiedBy { get; set; }
    public int YearsOfExperience { get; set; }
    public int MinimumAge { get; set; }
    public int MaximumAge { get; set; }
    public DropdownOptions? Sector { get; set; } = default!;
    public DropdownOptions? Management { get; set; } = default!;
    public DropdownOptions? Department { get; set; } = default!;
    public DropdownOptions? JobCategory { get; set; } = default!;
    public DropdownOptions? Gender { get; set; } = default!;
    public DropdownOptions? WorkLocation { get; set; } = default!;
    public DropdownOptions? Major { get; set; } = default!;
    public DropdownOptions? SubMajor { get; set; } = default!;
    public DropdownOptions? WorkType { get; set; } = default!;
    public DropdownOptions? JobStatus { get; set; } = default!;
    public List<JobDegreeResponseDto>? Degrees { get; set; }
    public List<JobConditionResponseDto>? Conditions { get; set; }
    public List<JobSkillResponseDto>? Skills { get; set; }
    public List<JobResponsibilityResponseDto>? Responsibilities { get; set; }
    public List<JobRequiredAttachmentResponseDto>? RequiredAttachments { get; set; }
    public List<JobTabReviewNoteResponseDto>? TabReviewNotes { get; set; }
}
