using Application.Operation.Features.Employee.Dashboard.Queries.Employees;
using Application.Operation.Features.Employee.Dashboard.Queries.Export;
using Application.Operation.Features.Employee.Dashboard.Queries.Invitations;
using Application.Operation.Features.Employee.Dashboard.Queries.Jobs;
using Application.Operation.Features.Employee.Dashboard.Queries.Overview;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Security;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers.Employee;

[ApiController]
[Route("api/operations-dashboard")]
[Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class OperationsDashboardController(IMediator mediator) : ControllerBase
{
    [HttpGet("overview")]
    [AuthorizePermission(PermissionKeys.Dashboard.View)]
    public async Task<IActionResult> GetOverview([FromQuery] GetDashboardOverviewQuery request, CancellationToken ct)
    {
        var result = await mediator.Send(request, ct);
        return result.ToActionResult();
    }

    [HttpGet("jobs/latest")]
    [AuthorizePermission(PermissionKeys.Dashboard.View)]
    public async Task<IActionResult> GetLatestJobs([FromQuery] GetLatestJobsQuery request, CancellationToken ct)
    {
        var result = await mediator.Send(request, ct);
        return result.ToActionResult();
    }

    [HttpGet("invitations/latest")]
    [AuthorizePermission(PermissionKeys.Dashboard.View)]
    public async Task<IActionResult> GetLatestInvitations([FromQuery] GetLatestInvitationsQuery request, CancellationToken ct)
    {
        var result = await mediator.Send(request, ct);
        return result.ToActionResult();
    }

    [HttpGet("team-performance")]
    [AuthorizePermission(PermissionKeys.Dashboard.View)]
    public async Task<IActionResult> GetTeamPerformance([FromQuery] GetTeamPerformanceQuery request, CancellationToken ct)
    {
        var result = await mediator.Send(request, ct);
        return result.ToActionResult();
    }

    [HttpGet("export-list")]
    [AuthorizePermission(PermissionKeys.Dashboard.View)]
    public async Task<IActionResult> ExportList([FromQuery] ExportDashboardListQuery request, CancellationToken ct)
    {
        var authorized = request.Context switch
        {
            DashboardExportContext.Candidates =>
                HasPermission(PermissionKeys.ProfileDistribution.View) ||
                HasPermission(PermissionKeys.ProfileApproval.View) ||
                HasPermission(PermissionKeys.ProfileApproval.Review),
            DashboardExportContext.Jobs =>
                HasPermission(PermissionKeys.Jobs.View) || HasPermission(PermissionKeys.Jobs.Edit),
            DashboardExportContext.Employees => HasPermission(PermissionKeys.ProfileDistribution.View),
            DashboardExportContext.Invitations => HasPermission(PermissionKeys.JobsInvitations.View),
            _ => false
        };
        if (!authorized) return Forbid();

        var result = await mediator.Send(request, ct);
        return result.IsFailed
            ? result.ToActionResult()
            : File(result.Value.Content, result.Value.ContentType, result.Value.FileName);
    }

    private bool HasPermission(string permission) =>
        User.HasClaim(RoleClaimTypes.Permission, permission);
}

