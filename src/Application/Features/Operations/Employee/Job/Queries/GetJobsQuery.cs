using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Queries;

public record GetJobsQuery : IQuery<IResult<PaginatedResult<JobResponseDto>>>
{
    public JobQueryFilter? Filter { get; init; }
    public PaginatedRequest Pagination { get; init; } = new PaginatedRequest();
}
