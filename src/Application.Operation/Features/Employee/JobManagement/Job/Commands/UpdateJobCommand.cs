using Application.Operation.Features.Employee.JobManagement.Job.DTOs;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Operation.Features.Employee.JobManagement.Job.Commands;

public record UpdateJobCommand(Guid JobId, UpdateJobDto Job) : ICommand<IResult<Unit>>;
