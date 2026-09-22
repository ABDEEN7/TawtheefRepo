using Application.Operation.Features.Employee.Interview.Evaluation.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Security;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers.Interview;

// Read-only "Start Interview" session browsing for the Evaluation stage, gated by
// InterviewEvaluation.{View,Manage} instead of InterviewSchedule.{View,Manage} - a committee member
// who only holds the evaluation permission (the intended audience for this stage, see
// EvaluationAccessResolver's own comment) can browse only their own committees' sessions here, while
// InterviewScheduleController stays the unscoped, InterviewSchedule-permission-gated surface for HR.
[Route("api/[controller]")]
[ApiController]
[Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class InterviewEvaluationSessionController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [AuthorizePermission(PermissionKeys.InterviewEvaluation.View, PermissionKeys.InterviewEvaluation.Manage)]
    public async Task<IActionResult> ListMySessions(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new ListMySessionsQuery(), cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("{id:guid}")]
    [AuthorizePermission(PermissionKeys.InterviewEvaluation.View, PermissionKeys.InterviewEvaluation.Manage)]
    public async Task<IActionResult> GetMySession(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetMySessionQuery(id), cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("{id:guid}/appointments")]
    [AuthorizePermission(PermissionKeys.InterviewEvaluation.View, PermissionKeys.InterviewEvaluation.Manage)]
    public async Task<IActionResult> ListMySessionAppointments(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new ListMySessionAppointmentsQuery(id), cancellationToken);
        return result.ToActionResult();
    }
}
