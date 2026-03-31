namespace Application.Recruitment.Features.Dashboard.DTOs;

public class InvitationAttachmentDto
{
    public Guid Id { get; set; }
    public Guid ResourceId { get; set; }
    public required string AttachmentTitleEn { get; set; }
    public required string AttachmentTitleAr { get; set; }
}
