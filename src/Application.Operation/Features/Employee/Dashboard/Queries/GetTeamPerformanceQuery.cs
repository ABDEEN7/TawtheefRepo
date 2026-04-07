using Application.Operation.Features.Employee.Dashboard.DTOs;
using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Employee.Dashboard.Queries;

public sealed record GetTeamPerformanceQuery(
    DateTime? FromDateUtc = null,
    DateTime? ToDateUtc = null,
    Guid? DepartmentId = null,
    Guid? EmployeeId = null,
    string? Search = null
) : PaginatedRequest, IRequest<Result<PaginatedResult<TeamPerformanceRowDto>>>;
