using System.Text.Json;
using Application.Operation.Features.Employee.Interview.Evaluation.Services;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.ResultReport.Services;

// Auto-generation entry point, called (before SaveChanges, same transaction) from every action that
// can be the one that finishes a schedule: a member Submit that completes an appointment, a manual
// CompleteAppointmentEvaluation, recording an absent candidate, a Candidate Withdrawal operational
// issue, and cancelling an appointment.
// Fires the whole schedule's InterviewResultReport exactly once, the moment the LAST live appointment
// becomes ready for review. Every early exit here returns Ok(): the triggering action must never fail
// because the schedule/template isn't ready yet or isn't configured for a supported calculation
// method - this is a silent no-op, not a user-facing error.
public static class InterviewResultCalculationService
{
    public static async Task<Result> TryGenerateReportIfScheduleDoneAsync(
        IUnitOfWork unitOfWork, Guid interviewScheduleId, CancellationToken cancellationToken)
    {
        // Live-candidate rule (InterviewAppointment.IsLiveCandidate) is applied in memory, not in SQL:
        // callers run this before SaveChanges, and EF hands back the caller's tracked instance, so an
        // appointment the caller just cancelled still reads as Scheduled in the DB but Cancelled here -
        // it must drop out, not block generation.
        var liveAppointments = (await unitOfWork.GetEntityRepository<InterviewAppointment>().DbSet
            .Where(a => a.InterviewScheduleId == interviewScheduleId && a.InvitationId != null)
            .ToListAsync(cancellationToken))
            .Where(InterviewAppointment.IsLiveCandidateCompiled)
            .ToList();

        if (liveAppointments.Count == 0)
            return Result.Ok();

        if (liveAppointments.Any(a => !a.IsReadyForReview))
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
            // Absent or withdrawn: scored 0 and never qualified, whatever was scored before a withdrawal -
            // and even when the template has no qualification score (which would otherwise pass 0).
            if (appointment.IsClosedByAttendance)
            {
                report.Candidates.Add(InterviewResultCandidate.Create(
                    appointment.Id, 0, version.QualificationScore, isQualified: false,
                    CalculationMethod.AverageOfEvaluators, DateTime.UtcNow));
                continue;
            }

            var evaluations = await unitOfWork.GetEntityRepository<InterviewMemberEvaluation>().DbSet
                .AsNoTracking()
                .Include(e => e.CriterionScores)
                .Where(e => e.InterviewAppointmentId == appointment.Id && e.Status == MemberEvaluationStatus.Submitted)
                .ToListAsync(cancellationToken);

            var candidate = BuildCandidate(appointment.Id, version, evaluations);
            report.Candidates.Add(candidate);
        }

        var readyResult = report.MarkReadyForCommitteeReview();
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
}
