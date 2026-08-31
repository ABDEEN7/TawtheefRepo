using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.Recruitment;

[Table(nameof(InvitationException), Schema = Schemas.Hr)]
public class InvitationException : EventEntity
{
    public const int ReasonMaxLength = 2000;

    public Guid JobId { get; init; }
    public Job? Job { get; init; }

    public Guid ApplicantId { get; init; }
    public ApplicantUser? Applicant { get; init; }

    public Guid? InvitationId { get; set; }
    public Invitation? Invitation { get; set; }

    public required string Reason { get; init; }

    public string? CancellationReason { get; private set; }

    public Guid ProofResourceId { get; init; }
    public Resource? ProofResource { get; init; }

    public InvitationExceptionStatus Status { get; set; } = InvitationExceptionStatus.ReadyToSend;

    public void Cancel(string reason)
    {
        CancellationReason = reason;
        Status = InvitationExceptionStatus.Cancelled;
    }
}
