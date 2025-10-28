using System.Net.Mail;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.Applicant;

public class AdditionalAttachmentApplicant: EventEntity
{
    public required string FileName { get; set; }
    public Guid AttachmentId { get; set; }
    public Resource? Attachment { get; set; }
    
    public Guid UserId { get; set; }
    public ApplicantUser? User { get; set; }
}
