namespace Application.Operation.Features.Employee.JobManagement.Job.DTOs;

public class UpdateJobDto : CreateJobDto
{
    public Guid Id { get; set; }
    public Guid JobStatusId { get; set; }
    public DateTimeOffset? PublishAt { get; set; }
    public DateTimeOffset? CancelledAt { get; set; }

    public string? OverviewAr { get; set; }
    public string? OverviewEn { get; set; }
    public string? BenefitsAr { get; set; }
    public string? BenefitsEn { get; set; }
    public string? QualificationsDescriptionAr { get; set; }
    public string? QualificationsDescriptionEn { get; set; }
    public List<JobConditionRequestDto>? Conditions { get; set; }
    public List<JobResponsibilityRequestDto>? Responsibilities { get; set; }
    public List<JobSkillRequestDto>? Skills { get; set; }
    public List<JobRequiredAttachmentRequestDto>? RequiredAttachments { get; set; }
}
