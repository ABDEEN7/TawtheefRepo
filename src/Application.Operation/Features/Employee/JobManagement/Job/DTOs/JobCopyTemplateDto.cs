namespace Application.Operation.Features.Employee.Job.DTOs;

public class JobCopyTemplateDto
{
    public string? TitleAr { get; set; }
    public string? TitleEn { get; set; }
    public Guid SectorId { get; set; }
    public Guid ManagementId { get; set; }
    public Guid DepartmentId { get; set; }
    public int YearsOfExperience { get; set; }
    public Guid JobCategoryId { get; set; }
    public Guid WorkLocationId { get; set; }
    public Guid? GenderId { get; set; }
    public Guid MajorId { get; set; }
    public Guid? SubMajorId { get; set; }
    public Guid WorkTypeId { get; set; }
    public int? NumberOfVacancies { get; set; }
    public DateTimeOffset? ClosingDate { get; set; }
    public int MinimumAge { get; set; }
    public int MaximumAge { get; set; }
    public string? OverviewAr { get; set; }
    public string? OverviewEn { get; set; }
    public string? BenefitsAr { get; set; }
    public string? BenefitsEn { get; set; }
    public string? QualificationsDescriptionAr { get; set; }
    public string? QualificationsDescriptionEn { get; set; }
    public List<JobDegreeRequestDto>? Degrees { get; set; }
    public List<JobConditionRequestDto>? Conditions { get; set; }
    public List<JobResponsibilityRequestDto>? Responsibilities { get; set; }
    public List<JobSkillRequestDto>? Skills { get; set; }
    public List<JobRequiredAttachmentRequestDto>? RequiredAttachments { get; set; }
    public JobPointsCopyDto? JobPoints { get; set; }
}
