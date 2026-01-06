using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Commands;

public sealed record SaveJobPointsCommand(
    JobPointsMainRequestDto Request
) : ICommand<IResult<Unit>>;
