using Cortex.Mediator.Commands;
using FluentResults;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Commands;

public record CreateJobCommand(CreateJobDto Job) : ICommand<IResult<Guid>>;
