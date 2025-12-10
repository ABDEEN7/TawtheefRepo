using Tawtheef.Application.Features.Authenticator.DTOs.Responses;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileApprovals.DTOs;

public record ProfileApprovalDataDto
{
    public BasicInformationSnapshot BasicInformation { get; init; } = new();
    public IReadOnlyList<QualificationDto> Qualifications { get; init; } = Array.Empty<QualificationDto>();
    public IReadOnlyList<ExperienceDto> Experiences { get; init; } = Array.Empty<ExperienceDto>();
    public IReadOnlyList<TrainingCourseDto> TrainingCourses { get; init; } = Array.Empty<TrainingCourseDto>();
    public IReadOnlyList<AchievementDto> ProfessionalCertificatesAndAwards { get; init; } = Array.Empty<AchievementDto>();
    public IReadOnlyList<SkillDto> SkillsAndLanguages { get; init; } = Array.Empty<SkillDto>();
    public IReadOnlyList<LanguageDto> Languages { get; init; } = Array.Empty<LanguageDto>();
    public IReadOnlyList<AdditionalAttachmentDto> Attachments { get; init; } = Array.Empty<AdditionalAttachmentDto>();
    public FileRefDto? ProfilePhoto { get; init; }
}

public record BasicInformationSnapshot
{
    public string? FullNameAr { get; init; }
    public string? FullNameEn { get; init; }
    public string? NationalNumber { get; init; }
    public DateOnly? BirthDate { get; init; }
    public string? Nationality { get; init; }
    public string? Gender { get; init; }
    public string? Religion { get; init; }
    public string? MaritalStatus { get; init; }
    public int ChildrenCount { get; init; }
    public string? CandidateType { get; init; }
    public string? TargetEntity { get; init; }
    public FileRefDto? ResumeAttachment { get; init; }
    public FileRefDto? NationalCard { get; init; }
    public FileRefDto? ResidenceAddressCertificate { get; init; }
    public FileRefDto? BirthdayCertificate { get; init; }
    public FileRefDto? MarriageCertificate { get; init; }
}
