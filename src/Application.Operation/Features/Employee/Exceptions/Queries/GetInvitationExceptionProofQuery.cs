using Application.Operation.Features.Employee.Exceptions.DTOs;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Exceptions.Queries;

public sealed record GetInvitationExceptionProofQuery(Guid ExceptionId)
    : IRequest<IResult<InvitationExceptionProofFileDto>>;
