using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Application.Recruitment.Features.Profile.DTOs.SaveOperation;

public sealed class SaveProfilePrereqRequest
{
    public Guid CandidateTypeId { get; set; }
    public Guid TargetEntityId { get; set; }
    public DateOnly? QIDExpiry { get; set; }

    [RegularExpression(@"^[a-zA-Z0-9\s\,\.\-\(\)]+$", ErrorMessage = "File name contains invalid characters.")]
    public string? CvFileName { get; set; }
    [RegularExpression(@"^[a-zA-Z0-9\s\,\.\-\(\)]+$", ErrorMessage = "File name contains invalid characters.")]
    public string? IdFileName { get; set; }
    [RegularExpression(@"^[a-zA-Z0-9\s\,\.\-\(\)]+$", ErrorMessage = "File name contains invalid characters.")]
    public string? BirthCertificateFileName { get; set; }
    [RegularExpression(@"^[a-zA-Z0-9\s\,\.\-\(\)]+$", ErrorMessage = "File name contains invalid characters.")]
    public string? MarriageCertificateFileName { get; set; }
    [RegularExpression(@"^[a-zA-Z0-9\s\,\.\-\(\)]+$", ErrorMessage = "File name contains invalid characters.")]
    public string? MarriageCertFileName { get; set; }

    public IFormFile? CvFile { get; set; }
    public IFormFile? IdFile { get; set; }
    public IFormFile? BirthCertificateFile { get; set; }
    public IFormFile? MarriageCertificateFile { get; set; }
    public bool Submit { get; set; }
}
