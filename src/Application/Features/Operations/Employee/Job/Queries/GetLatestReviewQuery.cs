using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Queries;

public record GetLatestReviewQuery(Guid JobId) : IQuery<IResult<JobReviewResponseDto>>;
