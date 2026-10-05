using System.Text.Json;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.Schedule.Services;

// InterviewAppointment.ApplyAttendanceOutcome closes the appointment for an absent or withdrawn
// candidate. Both callers (the chair's attendance command and a Candidate Withdrawal operational
// issue) record that closure the same way, next to their own audit row.
public static class AttendanceClosureAudit
{
    public static async Task AddIfClosedAsync(
        IUnitOfWork unitOfWork, InterviewAppointment appointment, AppointmentStatus previousStatus,
        CancellationToken cancellationToken)
    {
        if (!appointment.IsClosedByAttendance || previousStatus == AppointmentStatus.Closed)
            return;

        await unitOfWork.GetEntityRepository<InterviewAuditLog>().AddAsync(new InterviewAuditLog
        {
            EntityType = nameof(InterviewAppointment),
            EntityId = appointment.Id,
            Action = InterviewAppointmentAuditActions.Closed,
            OldValues = JsonSerializer.Serialize(new { Status = previousStatus }),
            NewValues = JsonSerializer.Serialize(new { appointment.Status, appointment.AttendanceStatus, FinalScore = 0 }),
            Reason = appointment.AttendanceStatus.ToString()
        }, cancellationToken);
    }
}
