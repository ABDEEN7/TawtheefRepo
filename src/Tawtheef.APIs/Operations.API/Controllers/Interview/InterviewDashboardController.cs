using Application.Operation.Features.Employee.Interview.Dashboard.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Security;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers.Interview;

// Read-only. The endpoint gate is "holds any Interview permission whose data the dashboard shows";
// what each caller actually sees is decided per section by InterviewDashboardAccessResolver.
[Route("api/[controller]")]
[ApiController]
[Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[AuthorizePermission(
    PermissionKeys.InterviewEvaluationTemplate.View, PermissionKeys.InterviewEvaluationTemplate.Manage,
    PermissionKeys.InterviewCommittee.View, PermissionKeys.InterviewCommittee.Manage,
    PermissionKeys.InterviewSchedule.View, PermissionKeys.InterviewSchedule.Manage,
    PermissionKeys.InterviewEvaluation.View, PermissionKeys.InterviewEvaluation.Manage,
    PermissionKeys.InterviewResultReport.View, PermissionKeys.InterviewResultReport.Manage)]
public class InterviewDashboardController(IMediator mediator) : ControllerBase
{
    [HttpGet("overview")]
    public async Task<IActionResult> GetOverview([FromQuery] GetInterviewDashboardOverviewQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("schedules")]
    public async Task<IActionResult> ListSchedules([FromQuery] ListInterviewDashboardSchedulesQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("candidates")]
    public async Task<IActionResult> ListCandidates([FromQuery] ListInterviewDashboardCandidatesQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("results")]
    public async Task<IActionResult> ListResults([FromQuery] ListInterviewDashboardResultsQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("issues")]
    public async Task<IActionResult> ListIssues([FromQuery] ListInterviewDashboardIssuesQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("lookups")]
    public async Task<IActionResult> ListLookups([FromQuery] ListInterviewDashboardLookupsQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }
}
