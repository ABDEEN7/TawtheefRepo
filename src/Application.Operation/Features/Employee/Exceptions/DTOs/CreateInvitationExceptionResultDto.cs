using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Operation.Features.Employee.Exceptions.DTOs;

public sealed record CreateInvitationExceptionResultDto(
    Guid ExceptionId,
    InvitationExceptionStatus Status,
    Guid ApplicantId,
    Guid JobId);
