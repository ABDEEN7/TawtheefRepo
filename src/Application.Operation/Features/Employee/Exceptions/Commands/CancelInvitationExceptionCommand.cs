using Application.Operation.Features.Employee.Exceptions.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Exceptions.Commands;

public sealed record CancelInvitationExceptionCommand(Guid ExceptionId, string Reason)
    : IRequest<IResult<CancelInvitationExceptionResultDto>>;
