using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.JobManagement.Job.DTOs;

public class JobResponseDto
{
    public Guid Id { get; set; }
    public Guid JobTitleId { get; set; }
    public string? TitleAr { get; set; } 
    public string? TitleEn { get; set; } 
    public string? JobNumber { get; set; }
    public int NumberOfVacancies { get; set; }
    public DateTimeOffset ClosingDate { get; set; }
    public string? BenefitsAr { get; set; } 
    public string? BenefitsEn { get; set; } 
    public string? OverViewAr { get; set; } 
    public string? OverViewEn { get; set; } 
    public string? QualificationDescriptionAr { get; set; }
    public string? QualificationDescriptionEn { get; set; } 
    public DateTimeOffset? PublishAt { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset? ModifiedDate { get; set; }
    public User? CreatedBy { get; set; }
    public User? ModifiedBy { get; set; }
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
    public byte[] RowVersion { get; set; } = [];
    public List<JobDegreeResponseDto>? Degrees { get; set; }
    public List<JobSpecializationResponseDto>? JobSpecializations { get; set; }
    public List<JobConditionResponseDto>? Conditions { get; set; }
    public List<JobSkillResponseDto>? Skills { get; set; }
    public List<JobResponsibilityResponseDto>? Responsibilities { get; set; }
    public List<JobRequiredAttachmentResponseDto>? RequiredAttachments { get; set; }
    public List<JobTabReviewNoteResponseDto>? TabReviewNotes { get; set; }
    public FileRefDto? ReviewAttachments { get; set; }
}
