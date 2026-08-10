using Application.Operation.Features.Employee.Dashboard.DTOs.Export;
using Application.Operation.Features.Employee.Dashboard.Queries.Common;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Dashboard.Queries.Export;

public enum DashboardExportContext { Candidates, Jobs, Employees, Invitations }

public sealed record ExportDashboardListQuery : DashboardQueryBase, IRequest<Result<DashboardExportResult>>
{
    public DashboardExportContext Context { get; init; }
    public string? Search { get; init; }
    public string? SortBy { get; init; }
    public string? SortDirection { get; init; }
}
