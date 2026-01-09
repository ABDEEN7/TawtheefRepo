using Microsoft.AspNetCore.Http;

namespace Tawtheef.Application.Features.Recruitment.Profile.DTOs;

public sealed class ReviseProfilePrereqAttachmentRequest
{
    public ReviseProfileAttachmentRequest? Birthday { get; set; }
    public IFormFile? BirthdayCertificate { get; set; }
    public ReviseProfileAttachmentRequest? Marriage { get; set; }
    public IFormFile? MarriageCertificate { get; set; }
}
