using Application.Operation.Features.Employee.JobManagement.Job.DTOs;
using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.JobManagement.Job.Commands;

public sealed record SaveJobReviewCommand(
    Guid JobId,
    SaveJobReviewRequestDto Request
) : IRequest<IResult<Unit>>;

