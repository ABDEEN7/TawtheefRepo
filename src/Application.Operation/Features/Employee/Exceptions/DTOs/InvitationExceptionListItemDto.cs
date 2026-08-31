using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Operation.Features.Employee.Exceptions.DTOs;

public sealed record InvitationExceptionListItemDto(
    Guid ExceptionId,
    Guid ApplicantId,
    string CandidateName,
    string Gender,
    string Qid,
    string Email,
    string PhoneNumber,
    Guid JobId,
    string JobNumber,
    string JobTitle,
    string Reason,
    InvitationExceptionStatus Status,
    Guid? InvitationId,
    Guid? InvitationStatusId,
    string? InvitationStatusName,
    DateTimeOffset CreatedDate,
    string? CancellationReason,
    bool HasProof);
