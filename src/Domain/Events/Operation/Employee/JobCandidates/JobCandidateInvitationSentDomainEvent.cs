using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Events.Operation.Employee.JobCandidates;

public sealed record JobCandidateInvitationSentDomainEvent(
    Guid InvitationId,
    Guid ApplicantId,
    string? Email,
    string? PhoneNumber,
    string JobTitle,
    int ExpiryDays,
    DateOnly ExpiresOn,
    DateTimeOffset DateOccurred) : BaseEvent(DateOccurred);
