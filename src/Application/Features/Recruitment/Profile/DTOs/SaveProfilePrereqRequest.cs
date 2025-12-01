using Microsoft.AspNetCore.Http;

namespace Tawtheef.Application.Features.Recruitment.Profile.DTOs;

public sealed class SaveProfilePrereqRequest
{
    public Guid CandidateTypeId { get; set; }
    public Guid TargetEntityId { get; set; }

    public string? CvFileName { get; set; }
    public string? IdFileName { get; set; }
    public string? BirthCertificateFileName { get; set; }
    public string? MarriageCertificateFileName { get; set; }
    public string? MarriageCertFileName { get; set; }

    public IFormFile? CvFile { get; set; }
    public IFormFile? IdFile { get; set; }
    public IFormFile? BirthCertificateFile { get; set; }
    public IFormFile? MarriageCertificateFile { get; set; }
    public bool Submit { get; set; }
}
