using Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.DTOs.Spanshot;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.DTOs.ProfileApproval;

public record ProfileApprovalDataDto
{
    public BasicInformationSnapshot BasicInformation { get; init; } = new();
    public IReadOnlyList<QualificationDto> Qualifications { get; set; } = [];
    public IReadOnlyList<ExperienceDto> Experiences { get; set; } = [];
    public IReadOnlyList<TrainingCourseDto> TrainingCourses { get; set; } = [];
    public IReadOnlyList<AchievementDto> ProfessionalCertificatesAndAwards { get; set; } = [];
    public IReadOnlyList<SkillDto> Skills { get; init; } = [];
    public IReadOnlyList<LanguageDto> Languages { get; init; } = [];
    public IReadOnlyList<AdditionalAttachmentDto> Attachments { get; set; } = [];
    public string? ProfilePhoto { get; set; }
}
