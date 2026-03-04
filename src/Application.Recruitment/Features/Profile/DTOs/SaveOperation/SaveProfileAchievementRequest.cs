using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Application.Recruitment.Features.Profile.DTOs.SaveOperation;

public sealed class SaveProfileAchievementRequest
{
    [RegularExpression(@"^[^\<\>]*$", ErrorMessage = "Invalid characters use in achievements.")]
    public string AchievementsJson { get; set; } = string.Empty;
    public List<IFormFile> AchievementFiles { get; set; } = [];
}

public sealed class AchievementUpsertDto
{
    public Guid? Id { get; set; }
    public Guid AchievementTypeId { get; set; }

    [RegularExpression(@"^[a-zA-Z0-9\u0600-\u06FF\s\,\.\-]+$", ErrorMessage = "Title contains invalid characters.")]
    public string Title { get; set; } = string.Empty;

    [RegularExpression(@"^[a-zA-Z0-9\u0600-\u06FF\s\,\.\-]+$", ErrorMessage = "Issuing authority contains invalid characters.")]
    public string IssuingAuthority { get; set; } = string.Empty;
    public Guid CountryId { get; set; }
    public DateOnly? IssueDate { get; set; }

    [RegularExpression(@"^[a-zA-Z0-9\u0600-\u06FF\s\,\.\-]+$", ErrorMessage = "Description contains invalid characters.")]
    public string? Description { get; set; }
    public bool? RelatedToSpecialization { get; set; }
    public Guid? AttachmentId { get; set; }
    public int? CertificateFileIndex { get; set; }
}

