using Microsoft.AspNetCore.Http;

namespace Tawtheef.Application.Features.Recruitment.Profile.DTOs;

public sealed class ReviseProfileContactAttachmentRequest
{
    public ReviseProfileAttachmentRequest? ResidenceAddress { get; set; }
    public IFormFile? ResidenceAddressCertificate { get; set; }
}