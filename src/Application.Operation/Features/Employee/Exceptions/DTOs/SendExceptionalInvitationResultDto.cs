using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Operation.Features.Employee.Exceptions.DTOs;

public sealed record SendExceptionalInvitationResultDto(
    Guid ExceptionId,
    Guid InvitationId,
    InvitationExceptionStatus ExceptionStatus,
    Guid InvitationStatusId,
    DateOnly ExpiresOn);
