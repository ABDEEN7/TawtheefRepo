using System.Text.Json.Serialization;
using Tawtheef.Application.Common.Models;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Authenticator.DTOs.Responses;

public record AuthResponse(
    bool RequiresProfileCompletion,
    UserInfoResponse? User = null,
    TokenResponse? Token = null
);

public sealed class ProfilePrefillDto
{
    public string? Email { get; init; }
    public bool EmailVerified { get; init; }
    public string? FullName { get; init; }
    public string? Avatar { get; init; }
    public string? Phone { get; init; }
    public bool PhoneVerified { get; init; }
    public string? Nationality { get; init; }
    public string? Qid { get; init; }
    public DateOnly? QidExpiry { get; init; }
    public string? Locale { get; init; }
    public string? Provider { get; init; }
}

public sealed class ProfileStatusDto
{
    public bool IsComplete { get; init; }
    public bool AgreedToTerms { get; init; }
    public UserProfileStatus Status { get; init; }
    public bool AvailableForRecruitment { get; init; }

    // ===== Scalars من UserProfile =====
    public Guid? CandidateTypeId { get; init; }
    public DropdownOptions? CandidateType { get; init; }
    public Guid? TargetEntityId { get; init; }
    public DropdownOptions? TargetEntity { get; init; }
    public Guid? OfficeId { get; init; }
    public DropdownOptions? Office { get; init; }

    public string? Avatar { get; init; }
    public string? FullNameAr { get; init; }
    public string? FullNameEn { get; init; }
    public string? Email { get; init; }
    public bool EmailVerified { get; init; }
    public string? Phone { get; init; }
    public bool PhoneVerified { get; init; }

    public string? NationalNumber { get; init; }
    public DateOnly? QIDExpiry { get; init; }
    public DateOnly? BirthDate { get; init; }
    public Guid? NationalityId { get; init; }
    public DropdownOptions? Nationality { get; init; }
    public Guid? GenderId { get; init; }
    public DropdownOptions? Gender { get; init; }
    
    public Guid? ReligionId { get; init; }
    public DropdownOptions? Religion { get; init; }
    public Guid? MaritalStatusId { get; init; }
    public DropdownOptions? MaritalStatus { get; init; }

    public int ChildrenCount { get; init; }

    public Guid? ResidenceCountryId { get; init; }
    public DropdownOptions? ResidenceCountry { get; init; }
    public Guid? InterviewLocationId { get; init; }
    public DropdownOptions? InterviewLocation { get; init; }

    public string? Address { get; init; }
    public int? naZone { get; init; }
    public int? naStreet { get; init; }
    public int? naBuilding { get; init; }
    public int? naUnit { get; init; }

    public bool HasDisability { get; init; }
    public string? DisabilityDetails { get; init; }
    
    public string? SponsorEmployerName { get; set; }
    public string? SponsorEmployerNumber { get; set; }
    public DateOnly? SponsorQidExpiry { get; set; }
    public FileRefDto? SponsorCard { get; set; }
    public Guid? SponsorTypeId { get; set; }
    public DropdownOptions? SponsorType { get; set; }

    // ===== Attachments كـ objects جاهزة للـ UI =====
    public FileRefDto? ResumeAttachment { get; init; }
    public FileRefDto? NationalCard { get; init; }
    public FileRefDto? ResidenceAddressCertificate { get; init; }
    public FileRefDto? BirthdayCertificate { get; init; }
    public FileRefDto? MarriageCertificate { get; init; }
    public IReadOnlyList<AdditionalAttachmentDto>? AdditionalAttachments { get; init; }

    // ===== Collections كاملة =====
    public IReadOnlyList<QualificationDto>? Qualifications { get; init; }
    public IReadOnlyList<ExperienceDto>? Experiences { get; init; }
    public IReadOnlyList<TrainingCourseDto>? TrainingCourses { get; init; }
    public IReadOnlyList<AchievementDto>? Achievements { get; init; }
    public IReadOnlyList<SkillDto>? Skills { get; init; }
    public IReadOnlyList<LanguageDto>? Languages { get; init; }
}
public sealed class FileRefDto
{
    public Guid ResourceId { get; init; }
    public string FileName { get; init; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}

// ====== Qualifications ======
public sealed class QualificationDto
{
    public Guid Id { get; init; }
    public Guid? DegreeId { get; init; }
    public DropdownOptions? Degree { get; init; }
    public Guid? GradCountryId { get; init; }
    public DropdownOptions? GradCountry { get; init; }
    
    public Guid? MajorId { get; init; }
    public DropdownOptions? Major { get; init; }
    public Guid? SubMajorId { get; init; }
    public DropdownOptions? SubMajor { get; init; }
    public Guid? UniversityId { get; init; }
    public DropdownOptions? University { get; init; }
    public Guid? StudyTypeId { get; init; }
    public DropdownOptions? StudyType { get; init; }
    public Guid? GradeId { get; init; }
    public DropdownOptions? Grade { get; init; }
    
    public int? GraduationYear { get; init; }
    public decimal? Gpa { get; init; }
    public FileRefDto? Attachment { get; set; }
}

// ====== Experiences ======
public sealed class ExperienceDto
{
    public Guid Id { get; init; }
    public string? Description { get; init; }
    public string? EmployerName { get; init; }
    public string? JobTitle { get; init; }
    public Guid? CountryId { get; init; }
    public DropdownOptions? Country { get; init; }
    public DateOnly? StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
    public bool IsCurrent { get; init; }
    public Guid? QualificationId { get; init; }
    public DropdownOptions? Qualification { get; init; }
    [JsonIgnore]
    public string? DegreeName { get; init; }
    [JsonIgnore]
    public string? MajorName { get; init; }
    [JsonIgnore]
    public string? UniversityName { get; init; }

    public FileRefDto? Attachment { get; set; }
}

// ====== Training Courses ======
public sealed class TrainingCourseDto
{
    public Guid Id { get; init; }
    public string? Title { get; init; }
    public string? Provider { get; init; }
    public Guid? CountryId { get; init; }
    public DropdownOptions? Country { get; init; }
    public DateOnly? StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
    public string? Description { get; init; }

    public FileRefDto? Attachment { get; set; }
}

// ====== Achievements ======
public sealed class AchievementDto
{
    public Guid Id { get; init; }
    public Guid AchievementTypeId { get; init; }
    public DropdownOptions? AchievementType { get; init; }
    public string? Title { get; init; }
    public string? IssuingAuthority { get; init; }
    public Guid? CountryId { get; init; }
    public DropdownOptions? Country { get; init; }
    public DateOnly? IssueDate { get; init; }
    public string? Description { get; init; }
    public bool? RelatedToSpecialization { get; init; }
    public FileRefDto? Attachment { get; set; }
}

// ====== Skills ======
public sealed class SkillDto
{
    public Guid Id { get; init; }
    public Guid SkillId { get; init; }
    public DropdownOptions? Skill { get; init; }
    public Guid LevelId { get; init; }
    public DropdownOptions? Level { get; init; }
}

// ====== Languages ======
public sealed class LanguageDto
{
    public Guid Id { get; init; }
    public Guid LanguageId { get; init; }
    public DropdownOptions? Language { get; init; }
    public Guid SpeakingLevelId { get; init; }
    public DropdownOptions? SpeakingLevel { get; init; }
    public Guid WritingLevelId { get; init; }
    public DropdownOptions? WritingLevel { get; init; }
    public Guid ReadingLevelId { get; init; }
    public DropdownOptions? ReadingLevel { get; init; }
}

// ====== Additional Attachments ======
public sealed class AdditionalAttachmentDto
{
    public Guid Id { get; init; }
    public string? Title { get; init; }
    public FileRefDto? File { get; set; } = new();
}

public sealed class ResidenceAddressDto
{
    
    public int BuildingNo { get; set; }
    public int StreetNo { get; set; }
    public int ZoneNo { get; set; }
    public int UnitNo { get; set; }
    
    public Guid ResidenceAddressCertificateId { get; set; }
    public FileRefDto? ResidenceAddressCertificate { get; set; }
}
