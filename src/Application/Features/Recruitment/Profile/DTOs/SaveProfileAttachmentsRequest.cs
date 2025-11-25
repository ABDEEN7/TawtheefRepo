namespace Tawtheef.Application.Features.Recruitment.Profile.DTOs;

public sealed class SaveProfileAttachmentsRequest
{
    public bool Submit { get; set; }

    public List<AdditionalAttachmentUpsertDto> Attachments { get; set; } = [];
}

public sealed class AdditionalAttachmentUpsertDto
{
    public Guid? Id { get; set; }
    public string FileName { get; set; } = default!;
    public Guid AttachmentId { get; set; }
}
