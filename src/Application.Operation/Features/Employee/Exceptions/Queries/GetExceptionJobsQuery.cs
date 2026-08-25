using System.ComponentModel.DataAnnotations;
using Application.Operation.Features.Employee.Exceptions.DTOs;
using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Employee.Exceptions.Queries;

public sealed record GetExceptionJobsQuery
    : PaginatedRequest,
        IRequest<IResult<PaginatedResult<ExceptionJobLookupDto>>>
{
    [MaxLength(200)]
    public string? SearchTerm { get; init; }
}
