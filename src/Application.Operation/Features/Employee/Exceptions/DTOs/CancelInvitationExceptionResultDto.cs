using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Operation.Features.Employee.Exceptions.DTOs;

public sealed record CancelInvitationExceptionResultDto(
    Guid ExceptionId,
    InvitationExceptionStatus ExceptionStatus,
    Guid? InvitationId,
    Guid? InvitationStatusId);
