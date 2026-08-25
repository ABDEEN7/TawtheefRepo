using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Operation.Features.Employee.Dashboard.DTOs.Invitations;

public sealed class LatestInvitationDto
{
    public Guid InvitationId { get; init; }
    public InvitationSource Source { get; init; }
    public required string Title { get; init; }
    public required string Status { get; init; }
    public DateTime SentDate { get; init; }
    public string? ActionKey { get; init; }
}
