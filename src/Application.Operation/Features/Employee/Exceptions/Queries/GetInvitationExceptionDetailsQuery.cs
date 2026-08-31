using Application.Operation.Features.Employee.Exceptions.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Exceptions.Queries;

public sealed record GetInvitationExceptionDetailsQuery(Guid ExceptionId)
    : IRequest<IResult<InvitationExceptionDetailsDto>>;
