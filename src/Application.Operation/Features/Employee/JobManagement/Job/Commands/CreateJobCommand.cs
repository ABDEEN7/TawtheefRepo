using Application.Operation.Features.Employee.Job.DTOs;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Operation.Features.Employee.Job.Commands;

public record CreateJobCommand(CreateJobDto Job) : ICommand<IResult<Guid>>;
