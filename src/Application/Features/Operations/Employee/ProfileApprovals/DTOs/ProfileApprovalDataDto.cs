using Tawtheef.Application.Features.Authenticator.DTOs.Responses;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileApprovals.DTOs;

public record ProfileApprovalDataDto
{
    public BasicInformationSnapshot BasicInformation { get; init; } = new();
    public IReadOnlyList<QualificationDto> Qualifications { get; set; } = [];
    public IReadOnlyList<ExperienceDto> Experiences { get; set; } = [];
    public IReadOnlyList<TrainingCourseDto> TrainingCourses { get; set; } = [];
    public IReadOnlyList<AchievementDto> ProfessionalCertificatesAndAwards { get; set; } = [];
    public IReadOnlyList<SkillDto> SkillsAndLanguages { get; init; } = [];
    public IReadOnlyList<LanguageDto> Languages { get; init; } = [];
    public IReadOnlyList<AdditionalAttachmentDto> Attachments { get; set; } = [];
    public FileRefDto? ProfilePhoto { get; set; }
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
    public FileRefDto? ResumeAttachment { get; set; }
    public FileRefDto? NationalCard { get; set; }
    public FileRefDto? ResidenceAddressCertificate { get; set; }
    public FileRefDto? BirthdayCertificate { get; set; }
    public FileRefDto? MarriageCertificate { get; set; }
}
