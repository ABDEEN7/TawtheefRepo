using Application.Operation.Features.Employee.Exceptions.DTOs;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Operation.Features.Employee.Exceptions.Commands;

public sealed record CreateInvitationExceptionCommand(
    string Qid,
    Guid JobId,
    string Reason,
    IFormFile? Proof)
    : IRequest<IResult<CreateInvitationExceptionResultDto>>;
