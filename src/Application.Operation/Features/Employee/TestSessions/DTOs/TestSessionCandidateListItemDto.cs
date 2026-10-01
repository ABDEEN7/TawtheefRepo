using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Operation.Features.Employee.TestSessions.DTOs;

public sealed record TestSessionCandidateListItemDto(
    Guid InvitationId,
    string? CandidateNumber,
    string CandidateName,
    string? JobTitle,
    string? Nationality,
    string? Gender,
    string? HighestQualification,
    decimal? QualificationScore,
    InvitationSource Source,
    string EligibilityStatus,
    string? EligibilityReason);
