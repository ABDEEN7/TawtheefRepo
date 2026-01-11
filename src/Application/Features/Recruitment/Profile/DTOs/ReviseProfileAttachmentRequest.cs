namespace Tawtheef.Application.Features.Recruitment.Profile.DTOs;

public sealed class ReviseProfileAttachmentRequest
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
}