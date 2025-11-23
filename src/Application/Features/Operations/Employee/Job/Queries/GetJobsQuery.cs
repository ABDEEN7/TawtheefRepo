using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Queries;

public record GetJobsQuery : IRequest<Result<PaginatedResult<JobResponseDto>>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SortBy { get; init; }
    public string? SortDirection { get; init; } = "asc";
    
    public PaginatedRequest ToPaginatedRequest() => new()
    {
        PageNumber = PageNumber,
        PageSize = PageSize,
        SortBy = SortBy,
        SortDirection = SortDirection
    };
}
