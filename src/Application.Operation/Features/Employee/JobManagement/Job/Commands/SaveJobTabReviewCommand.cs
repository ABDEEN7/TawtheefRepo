using Application.Operation.Features.Employee.JobManagement.Job.DTOs;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Operation.Features.Employee.JobManagement.Job.Commands;

public sealed record SaveJobReviewCommand(
    Guid JobId,
    SaveJobReviewRequestDto Request
) : ICommand<IResult<Unit>>;
