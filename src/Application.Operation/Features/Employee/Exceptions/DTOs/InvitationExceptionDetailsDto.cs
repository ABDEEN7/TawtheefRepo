using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Operation.Features.Employee.Exceptions.DTOs;

public sealed record InvitationExceptionDetailsDto(
    Guid ExceptionId,
    InvitationExceptionCandidateDto Candidate,
    InvitationExceptionJobDto Job,
    string Reason,
    string? CancellationReason,
    InvitationExceptionStatus Status,
    DateTimeOffset CreatedDate,
    DateTimeOffset? UpdatedDate,
    Guid? InvitationId,
    Guid? InvitationStatusId,
    string? InvitationStatusName,
    InvitationExceptionProofDto? Proof);

public sealed record InvitationExceptionCandidateDto(
    Guid ApplicantId,
    string CandidateName,
    string Qid);

public sealed record InvitationExceptionJobDto(
    Guid JobId,
    string JobNumber,
    string JobTitle);

public sealed record InvitationExceptionProofDto(
    Guid ResourceId,
    string FileName,
    ulong Size,
    string ContentType);
