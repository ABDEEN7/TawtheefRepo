using Application.Operation.Features.Employee.Interview.Schedule.Commands;
using Application.Operation.Features.Employee.Interview.Schedule.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Security;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers.Interview;

[Route("api/[controller]")]
[ApiController]
[Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class InterviewScheduleController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [AuthorizePermission(PermissionKeys.InterviewSchedule.View, PermissionKeys.InterviewSchedule.Manage)]
    public async Task<IActionResult> ListSchedules([FromQuery] ListSchedulesQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("{id:guid}")]
    [AuthorizePermission(PermissionKeys.InterviewSchedule.View, PermissionKeys.InterviewSchedule.Manage)]
    public async Task<IActionResult> GetScheduleById(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetScheduleByIdQuery(id), cancellationToken);
        return result.ToActionResult();
    }

    // Feeds wizard step 1/2: job identity, its (already-created) Approved committee + roster, and
    // the eligible-candidate pool stats for the step-2 cards.
    [HttpGet("creation-context")]
    [AuthorizePermission(PermissionKeys.InterviewSchedule.View, PermissionKeys.InterviewSchedule.Manage)]
    public async Task<IActionResult> GetScheduleCreationContext([FromQuery] GetScheduleCreationContextQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    // Read-only dry run of periods/distribution for the wizard's live step 2/3 preview - nothing
    // is persisted. Body-bound (POST) since Periods is a list, not query-string friendly.
    [HttpPost("preview")]
    [AuthorizePermission(PermissionKeys.InterviewSchedule.View, PermissionKeys.InterviewSchedule.Manage)]
    public async Task<IActionResult> PreviewScheduleSlots([FromBody] PreviewScheduleSlotsQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("appointments")]
    [AuthorizePermission(PermissionKeys.InterviewSchedule.View, PermissionKeys.InterviewSchedule.Manage)]
    public async Task<IActionResult> ListScheduleAppointments([FromQuery] ListScheduleAppointmentsQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost]
    [AuthorizePermission(PermissionKeys.InterviewSchedule.Manage)]
    public async Task<IActionResult> CreateSchedule([FromBody] CreateScheduleCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut]
    [AuthorizePermission(PermissionKeys.InterviewSchedule.Manage)]
    public async Task<IActionResult> UpdateSchedule([FromBody] UpdateScheduleCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("submit")]
    [AuthorizePermission(PermissionKeys.InterviewSchedule.Manage)]
    public async Task<IActionResult> SubmitSchedule([FromBody] SubmitScheduleCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("approve")]
    [AuthorizePermission(PermissionKeys.InterviewSchedule.Manage)]
    public async Task<IActionResult> ApproveSchedule([FromBody] ApproveScheduleCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("return")]
    [AuthorizePermission(PermissionKeys.InterviewSchedule.Manage)]
    public async Task<IActionResult> ReturnSchedule([FromBody] ReturnScheduleCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("cancel")]
    [AuthorizePermission(PermissionKeys.InterviewSchedule.Manage)]
    public async Task<IActionResult> CancelSchedule([FromBody] CancelScheduleCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("ready-for-execution")]
    [AuthorizePermission(PermissionKeys.InterviewSchedule.Manage)]
    public async Task<IActionResult> MarkScheduleReadyForExecution([FromBody] MarkScheduleReadyForExecutionCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("start-execution")]
    [AuthorizePermission(PermissionKeys.InterviewSchedule.Manage)]
    public async Task<IActionResult> StartScheduleExecution([FromBody] StartScheduleExecutionCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("close")]
    [AuthorizePermission(PermissionKeys.InterviewSchedule.Manage)]
    public async Task<IActionResult> CloseSchedule([FromBody] CloseScheduleCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    // ---- Appointments ----

    // Also accepts InterviewEvaluation.Manage: these two actions are the chair's Attendance
    // Registration step in the Evaluation stage, so a committee Chair who holds only the evaluation
    // permission (not InterviewSchedule.Manage) must be able to call them.

    [HttpPut("appointments/attendance")]
    [AuthorizePermission(PermissionKeys.InterviewSchedule.Manage, PermissionKeys.InterviewEvaluation.Manage)]
    public async Task<IActionResult> RecordAppointmentAttendance([FromBody] RecordAppointmentAttendanceCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("appointments/start-interview")]
    [AuthorizePermission(PermissionKeys.InterviewSchedule.Manage, PermissionKeys.InterviewEvaluation.Manage)]
    public async Task<IActionResult> StartAppointmentInterview([FromBody] StartAppointmentInterviewCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("appointments/under-evaluation")]
    [AuthorizePermission(PermissionKeys.InterviewSchedule.Manage)]
    public async Task<IActionResult> MarkAppointmentUnderEvaluation([FromBody] MarkAppointmentUnderEvaluationCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("appointments/complete-evaluation")]
    [AuthorizePermission(PermissionKeys.InterviewSchedule.Manage)]
    public async Task<IActionResult> CompleteAppointmentEvaluation([FromBody] CompleteAppointmentEvaluationCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("appointments/close")]
    [AuthorizePermission(PermissionKeys.InterviewSchedule.Manage)]
    public async Task<IActionResult> CloseAppointment([FromBody] CloseAppointmentCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("appointments/cancel")]
    [AuthorizePermission(PermissionKeys.InterviewSchedule.Manage)]
    public async Task<IActionResult> CancelAppointment([FromBody] CancelAppointmentCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("appointments/reschedule")]
    [AuthorizePermission(PermissionKeys.InterviewSchedule.Manage)]
    public async Task<IActionResult> RescheduleAppointment([FromBody] RescheduleAppointmentCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("appointments/send-notification")]
    [AuthorizePermission(PermissionKeys.InterviewSchedule.Manage)]
    public async Task<IActionResult> SendAppointmentNotification([FromBody] SendAppointmentNotificationCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    // ---- Lookups ----

    // Manage only: only the wizard's Step 2 period dialog calls this, to populate the In-Person
    // room picker. Searchable + capped server-side rather than paginated (dropdown feed).
    [HttpGet("lookups/rooms")]
    [AuthorizePermission(PermissionKeys.InterviewSchedule.Manage)]
    public async Task<IActionResult> ListInterviewRooms([FromQuery] ListInterviewRoomsQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }
}
