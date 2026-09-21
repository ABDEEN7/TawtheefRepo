using Application.Operation.Features.Employee.Interview.Evaluation.Commands;
using Application.Operation.Features.Employee.Interview.Evaluation.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Security;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers.Interview;

[Route("api/[controller]")]
[ApiController]
[Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class InterviewMemberEvaluationController(IMediator mediator) : ControllerBase
{
    [HttpGet("form")]
    [AuthorizePermission(PermissionKeys.InterviewEvaluation.View, PermissionKeys.InterviewEvaluation.Manage)]
    public async Task<IActionResult> GetMemberEvaluationForm([FromQuery] Guid appointmentId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetMemberEvaluationFormQuery(appointmentId), cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("appointments/{appointmentId:guid}/summary")]
    [AuthorizePermission(PermissionKeys.InterviewEvaluation.View, PermissionKeys.InterviewEvaluation.Manage)]
    public async Task<IActionResult> ListAppointmentEvaluations(Guid appointmentId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new ListAppointmentEvaluationsQuery(appointmentId), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("draft")]
    [AuthorizePermission(PermissionKeys.InterviewEvaluation.Manage)]
    public async Task<IActionResult> SaveMemberEvaluationDraft([FromBody] SaveMemberEvaluationDraftCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("submit")]
    [AuthorizePermission(PermissionKeys.InterviewEvaluation.Manage)]
    public async Task<IActionResult> SubmitMemberEvaluation([FromBody] SubmitMemberEvaluationCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }
}
