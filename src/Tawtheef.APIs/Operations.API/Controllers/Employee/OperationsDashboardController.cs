using Application.Operation.Features.Employee.Dashboard.Queries;
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

    [HttpGet("candidates/status")]
    [AuthorizePermission(PermissionKeys.Dashboard.View)]
    public async Task<IActionResult> GetCandidateStatus(
        [FromQuery] GetCandidateStatusSummaryQuery request,
        CancellationToken ct)
    {
        var result = await mediator.Send(request, ct);
        return result.ToActionResult();
    }

    [HttpGet("candidates/types")]
    [AuthorizePermission(PermissionKeys.Dashboard.View)]
    public async Task<IActionResult> GetCandidateTypes(
        [FromQuery] GetCandidateTypeSummaryQuery request,
        CancellationToken ct)
    {
        var result = await mediator.Send(request, ct);
        return result.ToActionResult();
    }

    [HttpGet("jobs/summary")]
    [AuthorizePermission(PermissionKeys.Dashboard.View)]
    public async Task<IActionResult> GetJobsSummary([FromQuery] GetJobsSummaryQuery request, CancellationToken ct)
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

    [HttpGet("employees/indicators")]
    [AuthorizePermission(PermissionKeys.Dashboard.View)]
    public async Task<IActionResult> GetEmployeeIndicators(
        [FromQuery] GetEmployeeIndicatorsQuery request,
        CancellationToken ct)
    {
        var result = await mediator.Send(request, ct);
        return result.ToActionResult();
    }

    [HttpGet("employees/review-outcomes")]
    [AuthorizePermission(PermissionKeys.Dashboard.View)]
    public async Task<IActionResult> GetEmployeeReviewOutcomes(
        [FromQuery] GetEmployeeReviewOutcomesQuery request,
        CancellationToken ct)
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
}

