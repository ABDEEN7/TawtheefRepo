using Microsoft.AspNetCore.Http;

namespace Application.Recruitment.Features.Profile.DTOs.ReviseOperation;

public sealed class ReviseProfilePersonalAttachmentRequest
{
    public ReviseProfileAttachmentRequest? SponsorCard { get; set; }
    public IFormFile? SponsorCardAttachment { get; set; }
    public ReviseProfileAttachmentRequest? Resume { get; set; }
    public IFormFile? ResumeAttachment { get; set; }
    public ReviseProfileAttachmentRequest? NationalCard { get; set; }
    public IFormFile? NationalCardAttachment { get; set; }
}
