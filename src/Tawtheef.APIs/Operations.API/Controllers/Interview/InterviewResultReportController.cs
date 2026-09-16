using Application.Operation.Features.Employee.Interview.ResultReport.Commands;
using Application.Operation.Features.Employee.Interview.ResultReport.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Security;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers.Interview;

[Route("api/[controller]")]
[ApiController]
[Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class InterviewResultReportController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [AuthorizePermission(PermissionKeys.InterviewResultReport.View, PermissionKeys.InterviewResultReport.Manage)]
    public async Task<IActionResult> ListResultReports([FromQuery] ListResultReportsQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("schedule/{scheduleId:guid}")]
    [AuthorizePermission(PermissionKeys.InterviewResultReport.View, PermissionKeys.InterviewResultReport.Manage)]
    public async Task<IActionResult> GetResultReportBySchedule(Guid scheduleId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetResultReportByScheduleQuery(scheduleId), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("approve")]
    [AuthorizePermission(PermissionKeys.InterviewResultReport.Manage)]
    public async Task<IActionResult> ApproveResultReport([FromBody] ApproveInterviewResultReportCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }
}
