using Application.Operation.Features.Employee.Dashboard.DTOs;
using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.Dashboard.Queries;

public sealed record GetOperationsDashboardQuery(
    DateTime? FromDateUtc = null,
    DateTime? ToDateUtc = null,
    Guid? DepartmentId = null,
    Guid? EmployeeId = null,
    string? Status = null
) : IRequest<Result<OperationsDashboardDto>>;

