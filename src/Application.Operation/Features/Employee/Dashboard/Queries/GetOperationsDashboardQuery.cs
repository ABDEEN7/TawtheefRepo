using Application.Operation.Features.Employee.Dashboard.DTOs;
using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Employee.Dashboard.Queries;

public sealed record GetOperationsDashboardQuery(
    Guid? CurrentUserId = null,
    string? CurrentRole = null,
    DateTime? FromDateUtc = null,
    DateTime? ToDateUtc = null,
    Guid? DepartmentId = null,
    Guid? EmployeeId = null,
    string? Status = null
) : PaginatedRequest, IQuery<Result<OperationsDashboardDto>>;
