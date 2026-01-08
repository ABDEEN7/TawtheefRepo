using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

namespace Tawtheef.Application.Features.Recruitment.JobDetails.DTOs;

public sealed class CandidateJobDetailsDto
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public int NumberOfVacancies { get; set; }
    public DateTime ClosingDate { get; set; }
    public string? Benefits { get; set; }
    public string? OverView { get; set; }
    public string? QualificationDescription { get; set; }
    public DateTimeOffset? PublishAt { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset? ModifiedDate { get; set; }
    public int YearsOfExperience { get; set; }
    public int MinimumAge { get; set; }
    public int MaximumAge { get; set; }
    public JobPointsMainResponseDto? JobPoints { get; set; }
    public DropdownOptions? Sector { get; set; }
    public DropdownOptions? Management { get; set; }
    public DropdownOptions? Department { get; set; }
    public DropdownOptions? JobCategory { get; set; }
    public DropdownOptions? Gender { get; set; }
    public DropdownOptions? WorkLocation { get; set; }
    public DropdownOptions? Major { get; set; }
    public DropdownOptions? SubMajor { get; set; }
    public DropdownOptions? WorkType { get; set; }
    public DropdownOptions? JobStatus { get; set; }
    public List<JobDegreeResponseDto>? Degrees { get; set; }
    public List<CandidateJobConditionDto>? Conditions { get; set; }
    public List<JobSkillResponseDto>? Skills { get; set; }
    public List<CandidateJobResponsibilityDto>? Responsibilities { get; set; }
    public List<CandidateJobRequiredAttachmentDto>? RequiredAttachments { get; set; }
}

public sealed class CandidateJobConditionDto
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public string? Text { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset? LastModifiedDate { get; set; }
}

public sealed class CandidateJobResponsibilityDto
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public string? Text { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset? LastModifiedDate { get; set; }
}

public sealed class CandidateJobRequiredAttachmentDto
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public string? Title { get; set; }
    public bool IsMandatory { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset? LastModifiedDate { get; set; }
}
