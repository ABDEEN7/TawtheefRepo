using Application.Operation.Features.Employee.Job.DTOs;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Operation.Features.Employee.Job.Commands;

public sealed record SaveJobPointsCommand(
    JobPointsMainRequestDto Request
) : ICommand<IResult<Unit>>;
