using System.Text.Json;
using Application.Operation.Features.Employee.Interview.Evaluation.Services;
using Application.Operation.Features.Employee.Interview.ResultReport.Services;
using Application.Operation.Features.Employee.Interview.Schedule.Commands;
using Application.Operation.Features.Employee.Interview.Schedule.Services;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.Schedule.Handlers.Commands;

public sealed class RecordAppointmentAttendanceCommandHandler(IUnitOfWork unitOfWork, EvaluationAccessResolver accessResolver)
    : IRequestHandler<RecordAppointmentAttendanceCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(RecordAppointmentAttendanceCommand request, CancellationToken cancellationToken)
    {
        // The chair records only Present or Absent (NoShow). Withdrew and Late stay valid attendance
        // outcomes, but they are recorded through operational issues (CreateOperationalIssueCommand).
        if (request.AttendanceStatus is not (AttendanceStatus.Present or AttendanceStatus.NoShow))
            return Result.Fail<Unit>(new Error(ErrorsCodes.InterviewAppointmentAttendanceStatusNotAllowed));

        var appointment = await unitOfWork.GetEntityRepository<InterviewAppointment>().DbSet
            .FirstOrDefaultAsync(a => a.Id == request.AppointmentId, cancellationToken);
        if (appointment is null)
            return Result.Fail<Unit>(new Error(ErrorsCodes.InterviewAppointmentNotFound));

        var accessResult = await accessResolver.EnsureChairOrBypassAsync(appointment.InterviewCommitteeId, cancellationToken);
        if (accessResult.IsFailed)
            return Result.Fail<Unit>(accessResult.Errors);

        var previousStatus = appointment.Status;
        var result = appointment.ApplyAttendanceOutcome(request.AttendanceStatus);
        if (result.IsFailed)
            return Result.Fail<Unit>(result.Errors);

        await unitOfWork.GetEntityRepository<InterviewAuditLog>().AddAsync(new InterviewAuditLog
        {
            EntityType = nameof(InterviewAppointment),
            EntityId = appointment.Id,
            Action = InterviewAppointmentAuditActions.AttendanceRecorded,
            NewValues = JsonSerializer.Serialize(new { request.AttendanceStatus })
        }, cancellationToken);

        await AttendanceClosureAudit.AddIfClosedAsync(unitOfWork, appointment, previousStatus, cancellationToken);

        // An absent candidate is now Closed (ready for review), so recording one can be what finishes
        // the schedule (e.g. the evaluated candidates were already Completed earlier).
        var generateResult = await InterviewResultCalculationService.TryGenerateReportIfScheduleDoneAsync(
            unitOfWork, appointment.InterviewScheduleId, cancellationToken);
        if (generateResult.IsFailed)
            return Result.Fail<Unit>(generateResult.Errors);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(Unit.Value);
    }
}
