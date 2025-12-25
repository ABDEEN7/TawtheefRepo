
using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Queries;

public record GetJobPointsConfigurationsByJobIdQuery(Guid JobId) : IRequest<IResult<JobPointConfigurationResponseDto>>;

