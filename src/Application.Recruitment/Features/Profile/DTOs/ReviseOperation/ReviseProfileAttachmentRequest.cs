namespace Application.Recruitment.Features.Profile.DTOs.ReviseOperation;

public sealed class ReviseProfileAttachmentRequest
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
}
