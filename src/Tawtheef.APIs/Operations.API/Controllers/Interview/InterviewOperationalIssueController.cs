using Application.Operation.Features.Employee.Interview.OperationalIssue.Commands;
using Application.Operation.Features.Employee.Interview.OperationalIssue.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Security;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers.Interview;

[Route("api/[controller]")]
[ApiController]
[Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class InterviewOperationalIssueController(IMediator mediator) : ControllerBase
{
    [HttpGet("appointments/{appointmentId:guid}")]
    [AuthorizePermission(PermissionKeys.InterviewSchedule.View, PermissionKeys.InterviewSchedule.Manage)]
    public async Task<IActionResult> ListAppointmentOperationalIssues(Guid appointmentId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new ListAppointmentOperationalIssuesQuery(appointmentId), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost]
    [AuthorizePermission(PermissionKeys.InterviewSchedule.Manage)]
    public async Task<IActionResult> CreateOperationalIssue([FromBody] CreateOperationalIssueCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("resolve")]
    [AuthorizePermission(PermissionKeys.InterviewSchedule.Manage)]
    public async Task<IActionResult> ResolveOperationalIssue([FromBody] ResolveOperationalIssueCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("waive")]
    [AuthorizePermission(PermissionKeys.InterviewSchedule.Manage)]
    public async Task<IActionResult> WaiveOperationalIssue([FromBody] WaiveOperationalIssueCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("blocking")]
    [AuthorizePermission(PermissionKeys.InterviewSchedule.Manage)]
    public async Task<IActionResult> UpdateOperationalIssueBlocking([FromBody] UpdateOperationalIssueBlockingCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }
}
