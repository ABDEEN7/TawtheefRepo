using Application.Operation.Features.Employee.Interview.EvaluationBank.DTOs;
using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Employee.Interview.EvaluationBank.Handlers.Queries;

public sealed record ListActiveAxesQuery : PaginatedRequest, IRequest<IResult<PaginatedResult<AxisDto>>>
{
    public string? Search { get; init; }
    public bool? IsActive { get; init; }
}
