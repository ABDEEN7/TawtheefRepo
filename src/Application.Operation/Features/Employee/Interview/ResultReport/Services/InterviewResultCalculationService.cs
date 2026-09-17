using System.Text.Json;
using Application.Operation.Features.Employee.Interview.Evaluation.Services;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.ResultReport.Services;

// Auto-generation entry point, called from SubmitMemberEvaluationCommandHandler right after it
// completes an appointment - fires the whole schedule's InterviewResultReport exactly once, the
// moment the LAST live appointment in the schedule reaches Completed. Every early exit here returns
// Ok(): a legitimate member Submit must never fail because the schedule/template isn't ready yet or
// isn't configured for a supported calculation method - this is a silent no-op, not a user-facing error.
public static class InterviewResultCalculationService
{
    public static async Task<Result> TryGenerateReportIfScheduleDoneAsync(
        IUnitOfWork unitOfWork, Guid interviewScheduleId, CancellationToken cancellationToken)
    {
        // "Live" = has a real candidate (not Held) and wasn't pulled out of the running (not
        // Cancelled/Rescheduled - a Rescheduled row is superseded by a new one, which is itself live).
        var liveAppointments = await unitOfWork.GetEntityRepository<InterviewAppointment>().DbSet
            .Where(a => a.InterviewScheduleId == interviewScheduleId
                && a.InvitationId != null
                && a.Status != AppointmentStatus.Cancelled
                && a.Status != AppointmentStatus.Rescheduled)
            .ToListAsync(cancellationToken);

        if (liveAppointments.Count == 0)
            return Result.Ok();

        if (liveAppointments.Any(a => !IsReadyForReview(a)))
            return Result.Ok();

        var alreadyExists = await unitOfWork.GetEntityRepository<InterviewResultReport>().DbSet
            .AnyAsync(r => r.InterviewScheduleId == interviewScheduleId, cancellationToken);
        if (alreadyExists)
            return Result.Ok();

        // One committee per schedule - every live appointment shares the same InterviewCommitteeId.
        var committeeId = liveAppointments[0].InterviewCommitteeId;
        var committee = await unitOfWork.GetEntityRepository<InterviewCommittee>().DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == committeeId, cancellationToken);
        if (committee is null)
            return Result.Ok();

        var versionResult = await EvaluationTemplateResolver.GetApprovedVersionAsync(unitOfWork, committee.InterviewTemplateId, cancellationToken);
        if (versionResult.IsFailed)
            return Result.Ok();
        var version = versionResult.Value;

        if (version.CalculationMethod != CalculationMethod.AverageOfEvaluators)
        {
            await unitOfWork.GetEntityRepository<InterviewAuditLog>().AddAsync(new InterviewAuditLog
            {
                EntityType = nameof(InterviewSchedule),
                EntityId = interviewScheduleId,
                Action = InterviewResultReportAuditActions.GenerationSkippedUnsupportedMethod,
                NewValues = JsonSerializer.Serialize(new { version.CalculationMethod })
            }, cancellationToken);
            return Result.Ok();
        }

        var report = InterviewResultReport.Create(interviewScheduleId, version.QualificationScore);

        foreach (var appointment in liveAppointments)
        {
            var evaluations = await unitOfWork.GetEntityRepository<InterviewMemberEvaluation>().DbSet
                .AsNoTracking()
                .Include(e => e.CriterionScores)
                .Where(e => e.InterviewAppointmentId == appointment.Id && e.Status == MemberEvaluationStatus.Submitted)
                .ToListAsync(cancellationToken);

            var candidate = BuildCandidate(appointment.Id, version, evaluations);
            report.Candidates.Add(candidate);
        }

        var readyResult = report.MarkReadyForReview();
        if (readyResult.IsFailed)
            return readyResult;

        await unitOfWork.GetEntityRepository<InterviewResultReport>().AddAsync(report, cancellationToken);

        await unitOfWork.GetEntityRepository<InterviewAuditLog>().AddAsync(new InterviewAuditLog
        {
            EntityType = nameof(InterviewResultReport),
            EntityId = report.Id,
            Action = InterviewResultReportAuditActions.Generated,
            NewValues = JsonSerializer.Serialize(new { interviewScheduleId, CandidateCount = report.Candidates.Count })
        }, cancellationToken);

        return Result.Ok();
    }

    // Per axis: sum each member's scores for that axis into a per-member subtotal, then average those
    // subtotals only across members who actually scored at least one criterion in the axis - a
    // SelectedAxes member who never touched this axis is excluded, not counted as a zero (which would
    // unfairly drag the average down). FinalScore is the sum of every axis's average.
    private static InterviewResultCandidate BuildCandidate(
        Guid appointmentId, InterviewTemplateVersion version, List<InterviewMemberEvaluation> evaluations)
    {
        decimal finalScore = 0;
        var axisRows = new List<InterviewResultCandidateAxis>();

        foreach (var axis in version.Axes)
        {
            var criterionIds = axis.Criteria.Select(c => c.Id).ToHashSet();

            var memberSubtotals = evaluations
                .Select(e => e.CriterionScores.Where(cs => criterionIds.Contains(cs.InterviewTemplateEvaluationCriterionId))
                    .Aggregate((decimal?)null, (sum, cs) => (sum ?? 0) + cs.Score))
                .Where(subtotal => subtotal is not null)
                .Select(subtotal => subtotal!.Value)
                .ToList();

            if (memberSubtotals.Count == 0)
                continue;

            var axisScore = memberSubtotals.Average();
            finalScore += axisScore;

            axisRows.Add(new InterviewResultCandidateAxis
            {
                InterviewTemplateEvaluationAxisId = axis.Id,
                Score = axisScore,
                QualificationScore = axis.QualificationScore,
                QualificationMet = axis.QualificationScore.HasValue ? axisScore >= axis.QualificationScore.Value : null
            });
        }

        var isQualified = !version.QualificationScore.HasValue || finalScore >= version.QualificationScore.Value;

        var candidate = InterviewResultCandidate.Create(
            appointmentId, finalScore, version.QualificationScore, isQualified,
            CalculationMethod.AverageOfEvaluators, DateTime.UtcNow);

        foreach (var axisRow in axisRows)
            candidate.Axes.Add(axisRow);

        return candidate;
    }

    // "Done" for final-review purposes, not just "evaluated" - StartInterview() requires
    // AttendanceStatus == Present, so a NoShow/Withdrew appointment can NEVER reach Completed on its
    // own. Without this, a schedule with even one such candidate would never generate a report at all,
    // even though that candidate has clearly "reached the point where their evaluation/review is
    // available" (there's simply nothing more to wait for). Late is deliberately excluded - it's a
    // transient marker, expected to be corrected to Present once the candidate actually arrives.
    private static bool IsReadyForReview(InterviewAppointment appointment) =>
        appointment.Status is AppointmentStatus.Completed or AppointmentStatus.Closed
        || appointment.AttendanceStatus is AttendanceStatus.NoShow or AttendanceStatus.Withdrew;
}
