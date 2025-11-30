using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Features.Operations.Employee.Job.Dtos;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Queries;

public record GetJobsQuery : PaginatedRequest, IRequest<IResult<PaginatedResult<JobResponseDto>>>
{
    public JobQueryFilter? Filter { get; init; }
    public PaginatedRequest? Pagination { get; init; }
}
