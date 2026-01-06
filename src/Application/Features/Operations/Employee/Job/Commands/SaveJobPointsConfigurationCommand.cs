using Cortex.Mediator.Commands;
using FluentResults;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Commands;

public record SaveJobPointsConfigurationCommand(JobPointConfigurationRequestDto Request) : ICommand<IResult<JobPointConfigurationResponseDto>>;
