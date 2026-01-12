using Application.Operation.Features.Employee.JobManagement.Job.DTOs;
using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Employee.JobManagement.Job.Queries;

public record GetJobsQuery : IQuery<IResult<PaginatedResult<JobResponseDto>>>
{
    public JobQueryFilter? Filter { get; init; }
    public PaginatedRequest Pagination { get; init; } = new PaginatedRequest();
}
