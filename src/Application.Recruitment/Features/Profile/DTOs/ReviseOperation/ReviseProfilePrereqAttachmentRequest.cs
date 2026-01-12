using Microsoft.AspNetCore.Http;

namespace Application.Recruitment.Features.Profile.DTOs.ReviseOperation;

public sealed class ReviseProfilePrereqAttachmentRequest
{
    public ReviseProfileAttachmentRequest? Birthday { get; set; }
    public IFormFile? BirthdayCertificate { get; set; }
    public ReviseProfileAttachmentRequest? Marriage { get; set; }
    public IFormFile? MarriageCertificate { get; set; }
}
