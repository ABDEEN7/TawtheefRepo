namespace Application.Operation.Features.Employee.MinisterOffice.DTOs;

public sealed class MinisterOfficeCandidateInvitationDto
{
    public Guid InvitationId { get; init; }
    public string JobTitleEn { get; init; } = default!;
    public string JobTitleAr { get; init; } = default!;
    public string InvitationStatus { get; init; } = default!;
    public DateTime? InvitedAt { get; init; }
}
