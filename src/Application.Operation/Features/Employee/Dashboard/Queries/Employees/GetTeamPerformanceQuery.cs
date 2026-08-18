using Application.Operation.Features.Employee.Dashboard.DTOs.Employees;
using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Employee.Dashboard.Queries.Employees;

public sealed record GetTeamPerformanceQuery(
    DateTime? FromDateUtc = null,
    DateTime? ToDateUtc = null,
    Guid? EmployeeId = null,
    string? Search = null,
    int? Year = null) : PaginatedRequest, IRequest<Result<PaginatedResult<TeamPerformanceRowDto>>>;
