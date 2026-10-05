using System.Text.Json;
using Application.Operation.Features.Employee.Interview.OperationalIssue.Commands;
using Application.Operation.Features.Employee.Interview.ResultReport.Services;
using Application.Operation.Features.Employee.Interview.Schedule.Services;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.OperationalIssue.Handlers.Commands;

public sealed class CreateOperationalIssueCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateOperationalIssueCommand, IResult<Guid>>
{
    public async Task<IResult<Guid>> Handle(CreateOperationalIssueCommand request, CancellationToken cancellationToken)
    {
        var appointment = await unitOfWork.GetEntityRepository<InterviewAppointment>().DbSet
            .FirstOrDefaultAsync(a => a.Id == request.AppointmentId, cancellationToken);
        if (appointment is null)
            return Result.Fail<Guid>(new Error(ErrorsCodes.InterviewAppointmentNotFound));

        // Some issues are also an attendance outcome, applied through the appointment's single closure
        // rule: a withdrawal closes the appointment (score 0), lateness only flags the candidate. If the
        // outcome isn't allowed (e.g. withdrawing an already Completed interview) the issue isn't logged.
        var previousStatus = appointment.Status;
        var outcome = AttendanceOutcomeFor(request.IssueType);
        if (outcome is not null)
        {
            var outcomeResult = appointment.ApplyAttendanceOutcome(outcome.Value);
            if (outcomeResult.IsFailed)
                return Result.Fail<Guid>(outcomeResult.Errors);
        }

        var issue = InterviewOperationalIssue.Create(
            request.AppointmentId, request.IssueType, request.Description, request.IsBlocking);
        await unitOfWork.GetEntityRepository<InterviewOperationalIssue>().AddAsync(issue, cancellationToken);

        await unitOfWork.GetEntityRepository<InterviewAuditLog>().AddAsync(new InterviewAuditLog
        {
            EntityType = nameof(InterviewOperationalIssue),
            EntityId = issue.Id,
            Action = InterviewOperationalIssueAuditActions.Created,
            NewValues = JsonSerializer.Serialize(new { request.AppointmentId, request.IssueType, request.IsBlocking })
        }, cancellationToken);

        if (outcome == AttendanceStatus.Withdrew)
        {
            await AttendanceClosureAudit.AddIfClosedAsync(unitOfWork, appointment, previousStatus, cancellationToken);

            // The withdrawn candidate is now ready for review, which can finish the schedule.
            var generateResult = await InterviewResultCalculationService.TryGenerateReportIfScheduleDoneAsync(
                unitOfWork, appointment.InterviewScheduleId, cancellationToken);
            if (generateResult.IsFailed)
                return Result.Fail<Guid>(generateResult.Errors);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(issue.Id);
    }

    private static AttendanceStatus? AttendanceOutcomeFor(OperationalIssueType issueType) => issueType switch
    {
        OperationalIssueType.CandidateWithdrawal => AttendanceStatus.Withdrew,
        OperationalIssueType.Late => AttendanceStatus.Late,
        _ => null
    };
}
