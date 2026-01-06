using Cortex.Mediator.Commands;
using FluentResults;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Commands;

public record CreateJobFromPreviousCommand(
    Guid SourceJobId,
    CreateJobFromPreviousDto Job) : ICommand<IResult<Guid>>;
