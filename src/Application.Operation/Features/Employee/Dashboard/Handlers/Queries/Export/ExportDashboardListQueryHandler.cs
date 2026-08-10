using Application.Operation.Features.Employee.Dashboard.DTOs.Export;
using Application.Operation.Features.Employee.Dashboard.Queries.Employees;
using Application.Operation.Features.Employee.Dashboard.Queries.Export;
using Application.Operation.Features.Employee.Dashboard.Services.Export;
using Application.Operation.Features.Employee.Dashboard.Services.Read;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.Dashboard.Handlers.Queries.Export;

internal sealed class ExportDashboardListQueryHandler(
    DashboardOverviewReader overviewReader,
    DashboardJobsReader jobsReader,
    DashboardInvitationsReader invitationsReader,
    DashboardEmployeesReader employeesReader,
    DashboardExcelExporter excelExporter)
    : IRequestHandler<ExportDashboardListQuery, Result<DashboardExportResult>>
{
    public Task<Result<DashboardExportResult>> Handle(ExportDashboardListQuery request, CancellationToken ct) =>
        request.Context switch
        {
            DashboardExportContext.Candidates => ExportCandidatesAsync(request, ct),
            DashboardExportContext.Jobs => ExportJobsAsync(request, ct),
            DashboardExportContext.Employees => ExportEmployeesAsync(request, ct),
            DashboardExportContext.Invitations => ExportInvitationsAsync(request, ct),
            _ => Task.FromResult(Result.Fail<DashboardExportResult>("Unsupported dashboard export context"))
        };

    private async Task<Result<DashboardExportResult>> ExportCandidatesAsync(ExportDashboardListQuery request, CancellationToken ct)
    {
        var result = await overviewReader.ReadAsync(request, ct);
        return result.IsFailed ? Result.Fail(result.Errors) : Result.Ok(excelExporter.ExportCandidates(result.Value));
    }

    private async Task<Result<DashboardExportResult>> ExportJobsAsync(ExportDashboardListQuery request, CancellationToken ct)
    {
        var result = await jobsReader.ReadLatestAsync(request, ct);
        return result.IsFailed ? Result.Fail(result.Errors) : Result.Ok(excelExporter.ExportJobs(result.Value));
    }

    private async Task<Result<DashboardExportResult>> ExportEmployeesAsync(ExportDashboardListQuery request, CancellationToken ct)
    {
        var overview = await overviewReader.ReadAsync(request, ct);
        if (overview.IsFailed) return Result.Fail(overview.Errors);
        var employees = await employeesReader.ReadExportAsync(new GetTeamPerformanceQuery(
            request.FromDateUtc, request.ToDateUtc, request.DepartmentId, request.EmployeeId, request.Search)
        {
            SortBy = request.SortBy,
            SortDirection = request.SortDirection
        }, ct);
        return employees.IsFailed
            ? Result.Fail(employees.Errors)
            : Result.Ok(excelExporter.ExportEmployees(overview.Value.Kpis, employees.Value));
    }

    private async Task<Result<DashboardExportResult>> ExportInvitationsAsync(ExportDashboardListQuery request, CancellationToken ct)
    {
        var result = await invitationsReader.ReadLatestAsync(request, ct);
        return result.IsFailed ? Result.Fail(result.Errors) : Result.Ok(excelExporter.ExportInvitations(result.Value));
    }
}
