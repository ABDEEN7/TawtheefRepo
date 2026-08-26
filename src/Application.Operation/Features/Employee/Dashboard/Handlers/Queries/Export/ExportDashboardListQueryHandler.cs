using Application.Operation.Features.Employee.Dashboard.Queries.Employees;
using Application.Operation.Features.Employee.Dashboard.Queries.Export;
using Application.Operation.Features.Employee.Dashboard.Services.Access;
using Application.Operation.Features.Employee.Dashboard.Services.Export;
using Application.Operation.Features.Employee.Dashboard.Services.Read;
using Application.Operation.Features.Employee.Dashboard.Services.Time;
using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models.Export;

namespace Application.Operation.Features.Employee.Dashboard.Handlers.Queries.Export;

internal sealed class ExportDashboardListQueryHandler(
    DashboardAccessContextProvider accessContextProvider,
    DashboardOverviewReader overviewReader,
    DashboardJobsReader jobsReader,
    DashboardInvitationsReader invitationsReader,
    DashboardEmployeesReader employeesReader,
    DashboardSummaryExcelExporter summaryExporter,
    DashboardCandidatesExcelExporter candidatesExporter,
    DashboardJobsExcelExporter jobsExporter,
    DashboardEmployeesExcelExporter employeesExporter,
    DashboardInvitationsExcelExporter invitationsExporter)
    : IRequestHandler<ExportDashboardListQuery, Result<FileExportResult>>
{
    public Task<Result<FileExportResult>> Handle(ExportDashboardListQuery request, CancellationToken ct) =>
        request.Context switch
        {
            DashboardExportContext.Summary => ExportSummaryAsync(request, ct),
            DashboardExportContext.Candidates => ExportCandidatesAsync(request, ct),
            DashboardExportContext.Jobs => ExportJobsAsync(request, ct),
            DashboardExportContext.Employees => ExportEmployeesAsync(request, ct),
            DashboardExportContext.Invitations => ExportInvitationsAsync(request, ct),
            _ => Task.FromResult(Result.Fail<FileExportResult>("Unsupported dashboard export context"))
        };

    private async Task<Result<FileExportResult>> ExportSummaryAsync(ExportDashboardListQuery request, CancellationToken ct)
    {
        var result = await overviewReader.ReadAsync(request, ct);
        if (result.IsFailed) return Result.Fail(result.Errors);

        var selectedYear = DashboardTemporalResolver.ResolveSelectedYear(
            request.Year, request.FromDateUtc);
        return Result.Ok(summaryExporter.Export(result.Value, selectedYear));
    }

    private async Task<Result<FileExportResult>> ExportCandidatesAsync(ExportDashboardListQuery request, CancellationToken ct)
    {
        var result = await overviewReader.ReadAsync(request, ct);
        return result.IsFailed ? Result.Fail(result.Errors) : Result.Ok(candidatesExporter.Export(result.Value));
    }

    private async Task<Result<FileExportResult>> ExportJobsAsync(
        ExportDashboardListQuery request,
        CancellationToken ct)
    {
        var result = await jobsReader.ReadExportAsync(request, ct);

        return result.IsFailed
            ? Result.Fail(result.Errors)
            : Result.Ok(jobsExporter.Export(result.Value));
    }

    private async Task<Result<FileExportResult>> ExportEmployeesAsync(ExportDashboardListQuery request, CancellationToken ct)
    {
        var contextResult = await accessContextProvider.GetAsync(ct);
        if (contextResult.IsFailed) return Result.Fail(contextResult.Errors);

        var context = contextResult.Value;
        var overview = await overviewReader.ReadAsync(request, context, ct);
        if (overview.IsFailed) return Result.Fail(overview.Errors);
        var employees = await employeesReader.ReadExportAsync(new GetTeamPerformanceQuery(
            request.FromDateUtc,
            request.ToDateUtc,
            request.EmployeeId,
            request.Search,
            request.Year)
        {
            SortBy = request.SortBy,
            SortDirection = request.SortDirection
        }, context, ct);
        return employees.IsFailed
            ? Result.Fail(employees.Errors)
            : Result.Ok(employeesExporter.Export(overview.Value.Kpis, employees.Value));
    }

    private async Task<Result<FileExportResult>> ExportInvitationsAsync(
        ExportDashboardListQuery request,
        CancellationToken ct)
    {
        var result = await invitationsReader.ReadExportAsync(
            request,
            ct);

        return result.IsFailed
            ? Result.Fail(result.Errors)
            : Result.Ok(invitationsExporter.Export(result.Value));
    }
}
