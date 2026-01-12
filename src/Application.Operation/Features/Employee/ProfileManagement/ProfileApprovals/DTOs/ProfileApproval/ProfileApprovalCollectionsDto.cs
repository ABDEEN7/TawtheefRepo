using System.Text.Json.Serialization;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;
using Tawtheef.Domain.Entities.Applicant;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.DTOs.ProfileApproval;

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
    public SpecializationRelationLevel? SpecializationRelation { get; init; }
    public Guid? QualificationId { get; init; }
    [JsonIgnore]
    public string? DegreeName { get; init; }
    [JsonIgnore]
    public string? MajorName { get; init; }
    [JsonIgnore]
    public string? UniversityName { get; init; }

    public FileRefDto? Attachment { get; set; }
}

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
    public SpecializationRelationLevel? SpecializationRelation { get; init; }

    public FileRefDto? Attachment { get; set; }
}

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

public sealed class SkillDto
{
    public Guid Id { get; init; }
    public Guid SkillId { get; init; }
    public DropdownOptions? Skill { get; init; }
    public Guid LevelId { get; init; }
    public DropdownOptions? Level { get; init; }
}

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
    public bool IsNative { get; init; }
}
