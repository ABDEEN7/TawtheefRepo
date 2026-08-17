using Application.Operation.Features.Employee.Dashboard.Queries.Common;
using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models.Export;

namespace Application.Operation.Features.Employee.Dashboard.Queries.Export;

public enum DashboardExportContext { Summary, Candidates, Jobs, Employees, Invitations }

public sealed record ExportDashboardListQuery : DashboardQueryBase, IRequest<Result<FileExportResult>>
{
    public DashboardExportContext Context { get; init; }
    public string? Search { get; init; }
    public string? SortBy { get; init; }
    public string? SortDirection { get; init; }
}
