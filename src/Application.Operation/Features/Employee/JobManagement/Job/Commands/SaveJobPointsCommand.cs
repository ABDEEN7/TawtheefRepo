using Application.Operation.Features.Employee.JobManagement.Job.DTOs;
using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.JobManagement.Job.Commands;

public sealed record SaveJobPointsCommand(
    JobPointsMainRequestDto Request
) : IRequest<IResult<Unit>>;

