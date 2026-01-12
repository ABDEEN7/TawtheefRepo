using Application.Operation.Features.Employee.Job.DTOs;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Operation.Features.Employee.Job.Commands;

public record CreateJobFromPreviousCommand(
    Guid SourceJobId,
    CreateJobFromPreviousDto Job) : ICommand<IResult<Guid>>;
