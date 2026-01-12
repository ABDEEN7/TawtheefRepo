using Microsoft.AspNetCore.Http;

namespace Application.Recruitment.Features.Profile.DTOs.ReviseOperation;

public sealed class ReviseProfileContactAttachmentRequest
{
    public ReviseProfileAttachmentRequest? ResidenceAddress { get; set; }
    public IFormFile? ResidenceAddressCertificate { get; set; }
}
