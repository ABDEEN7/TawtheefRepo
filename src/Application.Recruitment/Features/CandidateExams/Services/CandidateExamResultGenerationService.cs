using Application.Recruitment.Features.CandidateExams.Models;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Concurrency;
using Tawtheef.Application.Common.Interfaces.Concurrency;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Recruitment.Features.CandidateExams.Services;

public sealed class CandidateExamResultGenerationService(
    IUnitOfWork unitOfWork,
    CandidateExamResultCalculationService resultCalculationService,
    TimeProvider timeProvider,
    IApplicationLockService applicationLockService)
{
    public async Task<IReadOnlyList<Guid>> GetReadyExamIdsAsync(int take, CancellationToken cancellationToken)
    {
        if (take <= 0)
            return [];

        var resultCandidates = unitOfWork.Context.Set<ExamResultCandidate>();
        var unreportedCandidates = unitOfWork.Context.Set<TestSessionCandidate>()
            .AsNoTracking()
            .Where(x => x.StatusId != TestSessionCandidateStatusIds.Cancelled
                        && x.StatusId != TestSessionCandidateStatusIds.Rescheduled
                        && x.TestSession!.StatusId != TestSessionStatusIds.Cancelled
                        && !resultCandidates.Any(result => result.TestSessionCandidateId == x.Id));

        var initialReadyExamIds = unreportedCandidates
            .Where(x => x.TestSession!.StatusId == TestSessionStatusIds.Closed)
            .Select(x => x.TestSession!.ExamId)
            .Distinct()
            .Where(examId => !unreportedCandidates.Any(x =>
                x.TestSession!.ExamId == examId && x.TestSession.StatusId != TestSessionStatusIds.Closed));

        var returnedReadyExamIds = GetReadyReturnedReports().Select(report => report.ExamId);

        return await initialReadyExamIds.Union(returnedReadyExamIds)
            .OrderBy(examId => examId)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task ProcessReadyResultsAsync(Guid examId, CancellationToken cancellationToken)
    {
        if (examId == Guid.Empty)
            return;

        var reportIds = await GetReadyReturnedReports()
            .Where(report => report.ExamId == examId)
            .Select(report => report.Id)
            .OrderBy(id => id)
            .ToListAsync(cancellationToken);

        foreach (var reportId in reportIds)
        {
            await unitOfWork.ExecuteInTransactionAsync(async token =>
            {
                await applicationLockService.AcquireExclusiveAsync(ApplicationLockResources.CandidateExamResult(examId),
                    token);
                await applicationLockService.AcquireExclusiveAsync(
                    ApplicationLockResources.ExamResultReportWorkflow(reportId), token);
                return await ReprocessReturnedReportAsync(reportId, token);
            }, cancellationToken);
        }

        await unitOfWork.ExecuteInTransactionAsync(async token =>
        {
            await applicationLockService.AcquireExclusiveAsync(ApplicationLockResources.CandidateExamResult(examId),
                token);
            return await GenerateInternalAsync(examId, token);
        }, cancellationToken);
    }

    private IQueryable<ExamResultReport> GetReadyReturnedReports()
    {
        var openInterruptions = unitOfWork.Context.Set<TestAttemptInterruption>()
            .AsNoTracking()
            .Where(interruption => interruption.StatusId == TestAttemptInterruptionStatusIds.Open);
        var acceptedScoreInterruptions = unitOfWork.Context.Set<TestAttemptInterruption>()
            .AsNoTracking()
            .Where(interruption => interruption.StatusId == TestAttemptInterruptionStatusIds.Resolved
                                   && interruption.ResolutionActionId == TestAttemptInterruptionResolutionActionIds
                                       .AcceptCalculatedScore);

        return unitOfWork.Context.Set<ExamResultReport>()
            .AsNoTracking()
            .Where(report => report.StatusId == ExamResultReportStatusIds.Returned && report.Candidates.Any(candidate =>
                candidate.ResultStatusId == ExamResultCandidateResultStatusIds.RequiresAction
                && candidate.TestSessionCandidate != null
                && candidate.TestSessionCandidate.StatusId != TestSessionCandidateStatusIds.Cancelled
                && candidate.TestSessionCandidate.StatusId != TestSessionCandidateStatusIds.Rescheduled
                && candidate.TestSessionCandidate.TestSession!.StatusId == TestSessionStatusIds.Closed
                && ((candidate.TestSessionCandidate.RescheduledFromTestSessionCandidateId != null
                     && candidate.TestSessionCandidate.AttendanceStatusId
                     == TestSessionCandidateAttendanceStatusIds.NoShow
                     && candidate.TestSessionCandidate.TestAttempt == null)
                    || (candidate.TestSessionCandidate.TestAttempt != null
                        && (candidate.TestSessionCandidate.TestAttempt.StatusId == TestAttemptStatusIds.Completed
                            || candidate.TestSessionCandidate.TestAttempt.StatusId == TestAttemptStatusIds.TimedOut)
                        && (candidate.TestSessionCandidate.RescheduledFromTestSessionCandidateId != null
                            || (candidate.TestAttemptId == candidate.TestSessionCandidate.TestAttempt.Id
                                && acceptedScoreInterruptions.Any(interruption =>
                                    interruption.TestAttemptId == candidate.TestSessionCandidate.TestAttempt.Id)))
                        && !openInterruptions.Any(interruption =>
                            interruption.TestAttemptId == candidate.TestSessionCandidate.TestAttempt.Id)))));
    }

    private async Task<bool> ReprocessReturnedReportAsync(Guid reportId, CancellationToken cancellationToken)
    {
        var report = await unitOfWork.Context.Set<ExamResultReport>()
            .Include(x => x.Candidates)
            .SingleOrDefaultAsync(x => x.Id == reportId, cancellationToken);
        if (report is null || report.StatusId != ExamResultReportStatusIds.Returned)
            return false;

        var resultCandidates = unitOfWork.Context.Set<ExamResultCandidate>();
        var openInterruptions = unitOfWork.Context.Set<TestAttemptInterruption>()
            .AsNoTracking()
            .Where(x => x.StatusId == TestAttemptInterruptionStatusIds.Open);
        var acceptedScoreInterruptions = unitOfWork.Context.Set<TestAttemptInterruption>()
            .AsNoTracking()
            .Where(x => x.StatusId == TestAttemptInterruptionStatusIds.Resolved && x.ResolutionActionId
                == TestAttemptInterruptionResolutionActionIds.AcceptCalculatedScore);

        var assignments = await unitOfWork.Context.Set<TestSessionCandidate>()
            .AsNoTracking()
            .Where(x => resultCandidates.Any(candidate =>
                candidate.ExamResultReportId == reportId
                && candidate.ResultStatusId == ExamResultCandidateResultStatusIds.RequiresAction
                && candidate.TestSessionCandidateId == x.Id))
            .Select(x => new
            {
                x.Id,
                x.InvitationId,
                x.RescheduledFromTestSessionCandidateId,
                x.StatusId,
                x.AttendanceStatusId,
                x.TestSession!.ExamId,
                TestSessionStatusId = x.TestSession.StatusId,
                TestAttemptId = x.TestAttempt == null ? null : (Guid?)x.TestAttempt.Id,
                TestAttemptStatusId = x.TestAttempt == null ? null : (Guid?)x.TestAttempt.StatusId,
                HasOpenInterruption =
                    x.TestAttempt != null
                    && openInterruptions.Any(interruption => interruption.TestAttemptId == x.TestAttempt.Id),
                HasAcceptedCalculatedScore = x.TestAttempt != null && acceptedScoreInterruptions.Any(interruption =>
                    interruption.TestAttemptId == x.TestAttempt.Id)
            })
            .ToDictionaryAsync(x => x.Id, cancellationToken);

        var snapshotAt = timeProvider.GetUtcNow().UtcDateTime;
        var changed = false;

        var calculationRequests = new List<ReprocessCalculationRequest>();

        foreach (var candidate in report.Candidates.Where(x =>
                     x.ResultStatusId == ExamResultCandidateResultStatusIds.RequiresAction))
        {
            if (candidate.TestSessionCandidateId is not { } assignmentId)
                continue;

            if (!assignments.TryGetValue(assignmentId, out var assignment))
                throw new InvalidOperationException(
                    $"Current assignment {assignmentId} is missing for exam result candidate {candidate.Id}.");

            // Only the assignment currently linked to this result is eligible; earlier links are history.
            if (assignment.RescheduledFromTestSessionCandidateId is null && (!assignment.HasAcceptedCalculatedScore
                                                                             || candidate.TestAttemptId
                                                                             != assignment.TestAttemptId))
                continue;
            if (assignment.InvitationId != candidate.InvitationId || assignment.ExamId != report.ExamId)
                throw new InvalidOperationException(
                    $"Current assignment {assignmentId} does not match exam result candidate {candidate.Id}.");
            if (assignment.StatusId == TestSessionCandidateStatusIds.Cancelled
                || assignment.StatusId == TestSessionCandidateStatusIds.Rescheduled
                || assignment.TestSessionStatusId != TestSessionStatusIds.Closed)
                continue;

            if (assignment.AttendanceStatusId == TestSessionCandidateAttendanceStatusIds.NoShow
                && assignment.TestAttemptId is null)
            {
                candidate.MarkNoShow(snapshotAt);
                changed = true;
                continue;
            }

            if (assignment.TestAttemptId is not { } attemptId
                || (assignment.TestAttemptStatusId != TestAttemptStatusIds.Completed
                    && assignment.TestAttemptStatusId != TestAttemptStatusIds.TimedOut)
                || assignment.HasOpenInterruption)
                continue;

            calculationRequests.Add(new ReprocessCalculationRequest(candidate, attemptId));
        }

        if (calculationRequests.Count > 0)
        {
            var calculatedResults = await resultCalculationService.CalculateManyAsync(
                calculationRequests.Select(x => x.TestAttemptId).Distinct().ToArray(), cancellationToken);

            foreach (var request in calculationRequests)
            {
                if (!calculatedResults.TryGetValue(request.TestAttemptId, out var calculated)
                    || calculated.SpecializedQualificationThreshold != report.SpecializedQualificationThreshold)
                {
                    throw new InvalidOperationException(
                        $"Exam result calculation is inconsistent for report {report.Id}, "
                        + $"candidate {request.Candidate.Id}, " + $"attempt {request.TestAttemptId}.");
                }

                request.Candidate.ApplyCalculatedResult(
                    request.TestAttemptId,
                    calculated.SpecializedScore,
                    calculated.EducationalScore,
                    calculated.SkillsScore,
                    calculated.FinalScore,
                    calculated.IsSpecializedQualified,
                    snapshotAt);

                changed = true;
            }
        }

        if (!changed)
            return false;

        report.RecalculateRanking();
        if (report.Candidates.All(x => x.ResultStatusId != ExamResultCandidateResultStatusIds.RequiresAction))
        {
            var transitionAtUtc = timeProvider.GetUtcNow().UtcDateTime;
            var transition = report.MarkProcessingCompleted(transitionAtUtc);
            if (transition.IsFailed)
                throw new InvalidOperationException(
                    $"Exam result report {report.Id} could not return to review after reprocessing.");
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task<ExamResultReport?> GenerateInternalAsync(Guid examId, CancellationToken cancellationToken)
    {
        var resultCandidates = unitOfWork.Context.Set<ExamResultCandidate>();

        var openInterruptions = unitOfWork.Context.Set<TestAttemptInterruption>()
            .AsNoTracking()
            .Where(x => x.StatusId == TestAttemptInterruptionStatusIds.Open);

        var candidates = await unitOfWork.Context.Set<TestSessionCandidate>()
            .AsNoTracking()
            .Where(x => x.TestSession!.ExamId == examId && x.TestSession.StatusId != TestSessionStatusIds.Cancelled
                                                        && x.StatusId != TestSessionCandidateStatusIds.Cancelled
                                                        && x.StatusId != TestSessionCandidateStatusIds.Rescheduled
                                                        && !resultCandidates.Any(result =>
                                                            result.TestSessionCandidateId == x.Id))
            .Select(x => new CandidateExamResultGenerationCandidateSnapshot(x.Id, x.InvitationId, x.TestSessionId,
                x.TestSession!.StatusId, x.AttendanceStatusId, x.TestAttempt == null ? null : x.TestAttempt.Id,
                x.TestAttempt == null ? null : x.TestAttempt.StatusId,
                x.TestAttempt != null && openInterruptions.Any(interruption =>
                    interruption.TestAttemptId == x.TestAttempt.Id)))
            .ToListAsync(cancellationToken);

        if (candidates.Count == 0)
            return null;

        // We must not create the report while any candidate
        // in this unreported wave can still change.
        if (candidates.Any(x => x.TestSessionStatusId != TestSessionStatusIds.Closed))
        {
            return null;
        }

        // Rescheduling should leave only one active assignment
        // for the invitation.
        if (candidates.GroupBy(x => x.InvitationId).Any(x => x.Count() > 1))
        {
            return null;
        }

        var specializedQualificationThreshold = await unitOfWork.Context.Set<ExamPart>()
            .AsNoTracking()
            .Where(x => x.ExamId == examId && x.PartNo == 1)
            .Select(x => x.QualificationScore)
            .SingleOrDefaultAsync(cancellationToken);

        if (specializedQualificationThreshold is null || specializedQualificationThreshold is < 0 or > 100)
        {
            return null;
        }

        var snapshotAt = timeProvider.GetUtcNow().UtcDateTime;

        var calculableAttemptIds = candidates.Where(CanCalculateResult)
            .Select(candidate => candidate.TestAttemptId!.Value)
            .Distinct()
            .ToArray();

        var calculatedResults =
            await resultCalculationService.CalculateManyAsync(calculableAttemptIds, cancellationToken);

        var report = ExamResultReport.Create(examId, specializedQualificationThreshold.Value);

        foreach (var candidate in candidates)
        {
            var result = BuildCandidate(candidate, specializedQualificationThreshold.Value, snapshotAt,
                calculatedResults);

            if (result is null)
            {
                return null;
            }

            report.Candidates.Add(result);
        }

        report.RecalculateRanking();

        var transitionAtUtc = timeProvider.GetUtcNow().UtcDateTime;
        var readyResult = report.MarkReadyForReview(transitionAtUtc);

        if (readyResult.IsFailed)
            return null;

        await unitOfWork.Context.Set<ExamResultReport>().AddAsync(report, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return report;
    }

    private static bool CanCalculateResult(CandidateExamResultGenerationCandidateSnapshot candidate) =>
        candidate.AttendanceStatusId != TestSessionCandidateAttendanceStatusIds.NoShow
        && candidate.TestAttemptId.HasValue
        && (candidate.TestAttemptStatusId == TestAttemptStatusIds.Completed
            || candidate.TestAttemptStatusId == TestAttemptStatusIds.TimedOut) && !candidate.HasOpenInterruption;

    private static ExamResultCandidate? BuildCandidate(CandidateExamResultGenerationCandidateSnapshot candidate,
        decimal specializedQualificationThreshold, DateTime snapshotAt,
        IReadOnlyDictionary<Guid, CandidateExamCalculatedScores> calculatedResults)
    {
        if (candidate.AttendanceStatusId == TestSessionCandidateAttendanceStatusIds.NoShow)
        {
            if (candidate.TestAttemptId is not null)
            {
                return BuildRequiresAction(candidate, snapshotAt);
            }

            var noShowResult = new ExamResultCandidate
            {
                InvitationId = candidate.InvitationId,
                TestSessionCandidateId =
                    candidate.TestSessionCandidateId
            };

            noShowResult.MarkNoShow(snapshotAt);

            return noShowResult;
        }

        if (!CanCalculateResult(candidate))
        {
            return BuildRequiresAction(
                candidate,
                snapshotAt);
        }

        var testAttemptId = candidate.TestAttemptId!.Value;
        
        if (!calculatedResults.TryGetValue(testAttemptId, out var calculated))
        {
            return null;
        }

        if (calculated.SpecializedQualificationThreshold != specializedQualificationThreshold)
        {
            return null;
        }

        var result = new ExamResultCandidate
        {
            InvitationId = candidate.InvitationId,
            TestSessionCandidateId =
                candidate.TestSessionCandidateId
        };

        result.ApplyCalculatedResult(
            testAttemptId,
            calculated.SpecializedScore,
            calculated.EducationalScore,
            calculated.SkillsScore,
            calculated.FinalScore,
            calculated.IsSpecializedQualified,
            snapshotAt);

        return result;
    }

    private static ExamResultCandidate BuildRequiresAction(CandidateExamResultGenerationCandidateSnapshot candidate,
        DateTime snapshotAt)
    {
        return new ExamResultCandidate
        {
            InvitationId = candidate.InvitationId,
            TestSessionCandidateId = candidate.TestSessionCandidateId,
            TestAttemptId = candidate.TestAttemptId,
            SpecializedScore = null,
            EducationalScore = null,
            SkillsScore = null,
            FinalScore = null,
            Rank = null,
            ResultStatusId = ExamResultCandidateResultStatusIds.RequiresAction,
            SnapshotAt = snapshotAt
        };
    }

    private sealed record ReprocessCalculationRequest(
        ExamResultCandidate Candidate,
        Guid TestAttemptId);
}
