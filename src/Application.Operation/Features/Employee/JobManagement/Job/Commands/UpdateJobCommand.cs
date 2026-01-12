using Application.Operation.Features.Employee.Job.DTOs;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Operation.Features.Employee.Job.Commands;

public record UpdateJobCommand(Guid JobId, UpdateJobDto Job) : ICommand<IResult<Unit>>;
