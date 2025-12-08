using Microsoft.AspNetCore.Http;

namespace Tawtheef.Application.Features.Recruitment.Profile.DTOs;

public sealed class SaveProfileAchievementRequest
{
    public string AchievementsJson { get; set; } = string.Empty;
    public List<IFormFile> AchievementFiles { get; set; } = [];
}

public sealed class AchievementUpsertDto
{
    public Guid? Id { get; set; }
    public Guid AchievementTypeId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string IssuingAuthority { get; set; } = string.Empty;
    public Guid CountryId { get; set; }
    public DateOnly? IssueDate { get; set; }
    public string? Description { get; set; }
    public bool? RelatedToSpecialization { get; set; }
    public Guid? AttachmentId { get; set; }
    public int? CertificateFileIndex { get; set; }
}
