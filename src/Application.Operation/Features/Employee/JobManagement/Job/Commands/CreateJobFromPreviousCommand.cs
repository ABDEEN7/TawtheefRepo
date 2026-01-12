using Application.Operation.Features.Employee.JobManagement.Job.DTOs;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Operation.Features.Employee.JobManagement.Job.Commands;

public record CreateJobFromPreviousCommand(
    Guid SourceJobId,
    CreateJobFromPreviousDto Job) : ICommand<IResult<Guid>>;
