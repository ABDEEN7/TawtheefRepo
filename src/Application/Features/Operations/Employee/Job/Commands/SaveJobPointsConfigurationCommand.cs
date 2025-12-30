using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Commands;

public record SaveJobPointsConfigurationCommand(JobPointConfigurationRequestDto Request) : IRequest<IResult<JobPointConfigurationResponseDto>>;
