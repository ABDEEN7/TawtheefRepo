using Application.Operation.Features.Employee.Exams.DTOs;
using Application.Operation.Features.Employee.Exams.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Employee.Exams.Handlers.Queries;

public sealed class GetExamResultReportDetailsQueryHandler(
    IUnitOfWork unitOfWork)
    : IRequestHandler<
        GetExamResultReportDetailsQuery,
        IResult<ExamResultReportDetailsDto>>
{
    public async Task<IResult<ExamResultReportDetailsDto>> Handle(
        GetExamResultReportDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var report = await unitOfWork.Context
            .Set<ExamResultReport>()
            .AsNoTracking()
            .AsSplitQuery()
            .Include(x => x.Exam)
            .ThenInclude(x => x!.Job)
            .ThenInclude(x => x!.JobTitle)
            .Include(x => x.Status)
            .Include(x => x.Candidates)
            .ThenInclude(x => x.ResultStatus)
            .Include(x => x.Candidates)
            .ThenInclude(x => x.Invitation)
            .ThenInclude(x => x!.Applicant)
            .ThenInclude(x => x!.Profile)
            .Include(x => x.Candidates)
            .ThenInclude(x => x.TestSessionCandidate)
            .ThenInclude(x => x!.AttendanceStatus)
            .Include(x => x.Candidates)
            .ThenInclude(x => x.TestSessionCandidate)
            .ThenInclude(x => x!.IdentityVerificationStatus)
            .Include(x => x.Candidates)
            .ThenInclude(x => x.TestSessionCandidate)
            .ThenInclude(x => x!.Status)
            .Include(x => x.Candidates)
            .ThenInclude(x => x.TestSessionCandidate)
            .ThenInclude(x => x!.TestSession)
            .ThenInclude(x => x!.TestSlot)
            .Include(x => x.Candidates)
            .ThenInclude(x => x.TestAttempt)
            .ThenInclude(x => x!.Status)
            .FirstOrDefaultAsync(
                x => x.Id == request.ReportId,
                cancellationToken);

        if (report is null)
        {
            return Result.Fail<ExamResultReportDetailsDto>(
                new Error(
                    ErrorsCodes.ExamResultReportNotFound));
        }

        var attemptIds = report.Candidates
            .Where(x => x.TestAttemptId.HasValue)
            .Select(x => x.TestAttemptId!.Value)
            .Distinct()
            .ToList();

        var interruptions = attemptIds.Count == 0
            ? []
            : await unitOfWork.Context
                .Set<TestAttemptInterruption>()
                .AsNoTracking()
                .Where(x =>
                    attemptIds.Contains(x.TestAttemptId))
                .OrderBy(x => x.InterruptedAt)
                .Select(x => new InterruptionProjection(
                    x.Id,
                    x.TestAttemptId,
                    x.InterruptedAt,
                    x.Reason,
                    x.StatusId,
                    x.Status!.BackendName,
                    x.Status.NameAr,
                    x.Status.NameEn,
                    x.ResolutionActionId,
                    x.ResolutionAction == null
                        ? null
                        : x.ResolutionAction.BackendName,
                    x.ResolutionAction == null
                        ? null
                        : x.ResolutionAction.NameAr,
                    x.ResolutionAction == null
                        ? null
                        : x.ResolutionAction.NameEn,
                    x.ResolutionNotes,
                    x.ResolvedById,
                    x.ResolvedAt))
                .ToListAsync(cancellationToken);

        var interruptionsByAttempt =
            interruptions
                .GroupBy(x => x.TestAttemptId)
                .ToDictionary(
                    x => x.Key,
                    x => x.ToList());

        var candidates = report.Candidates
            .Select(candidate =>
            {
                var assignment =
                    candidate.TestSessionCandidate;

                var session =
                    assignment?.TestSession;

                var slot =
                    session?.TestSlot;

                var invitation =
                    candidate.Invitation;

                var applicant =
                    invitation?.Applicant;

                var attempt =
                    candidate.TestAttempt;

                var candidateInterruptions =
                    candidate.TestAttemptId.HasValue
                        ? interruptionsByAttempt.GetValueOrDefault(
                            candidate.TestAttemptId.Value,
                            [])
                        : [];

                bool? isSpecializedQualified =
                    candidate.SpecializedScore.HasValue
                        ? candidate.SpecializedScore.Value >=
                          report.SpecializedQualificationThreshold
                        : null;

                return new ExamResultReportCandidateDto(
                    candidate.Id,
                    candidate.InvitationId,
                    candidate.TestSessionCandidateId,
                    candidate.TestAttemptId,
                    applicant?.FullNameAr
                    ?? string.Empty,
                    applicant?.FullNameEn,
                    applicant?.Profile?.NationalNumber,
                    session?.SessionNo,
                    slot?.TitleAr,
                    slot?.TitleEn,
                    slot?.SlotDate,
                    slot?.StartTime,
                    slot?.EndTime,
                    ToLookup(
                        assignment?.AttendanceStatus),
                    ToLookup(
                        assignment?
                            .IdentityVerificationStatus),
                    ToLookup(
                        assignment?.Status),
                    ToLookup(
                        attempt?.Status),
                    attempt?.StartedAt,
                    attempt?.CompletedAt,
                    candidate.SpecializedScore,
                    isSpecializedQualified,
                    candidate.EducationalScore,
                    candidate.SkillsScore,
                    candidate.FinalScore,
                    candidate.Rank,
                    ToLookup(candidate.ResultStatus)!,
                    candidateInterruptions.Any(x =>
                        x.StatusId ==
                        TestAttemptInterruptionStatusIds.Open),
                    candidateInterruptions
                        .Select(x =>
                            new ExamResultInterruptionDto(
                                x.Id,
                                x.InterruptedAt,
                                x.Reason,
                                new ExamResultLookupDto(
                                    x.StatusId,
                                    x.StatusBackendName,
                                    x.StatusNameAr,
                                    x.StatusNameEn),
                                x.ResolutionActionId.HasValue
                                    ? new ExamResultLookupDto(
                                        x.ResolutionActionId.Value,
                                        x.ResolutionBackendName
                                        ?? string.Empty,
                                        x.ResolutionNameAr
                                        ?? string.Empty,
                                        x.ResolutionNameEn
                                        ?? string.Empty)
                                    : null,
                                x.ResolutionNotes,
                                x.ResolvedById,
                                x.ResolvedAt))
                        .ToList(),
                    candidate.SnapshotAt);
            })
            .OrderBy(x =>
                x.Rank.HasValue ? 0 : 1)
            .ThenBy(x => x.Rank)
            .ThenBy(x => x.CandidateNameAr)
            .ToList();

        var dto =
            new ExamResultReportDetailsDto(
                report.Id,
                report.ExamId,
                report.Exam!.ExamNo,
                report.Exam.TitleAr,
                report.Exam.TitleEn,
                report.Exam.JobId,
                report.Exam.Job!.JobTitle!.JobNameAr,
                report.Exam.Job.JobTitle.JobNameEn,
                report.SpecializedQualificationThreshold,
                ToLookup(report.Status)!,
                report.ApprovedById,
                report.ApprovedAt,
                report.ClosedAt,
                report.DecisionNotes,
                report.CreatedDate,
                candidates);

        return Result.Ok(dto);
    }

    private static ExamResultLookupDto? ToLookup(
        LookupBase? lookup)
    {
        if (lookup is null)
            return null;

        return new ExamResultLookupDto(
            lookup.Id,
            lookup.BackendName,
            lookup.NameAr,
            lookup.NameEn);
    }

    private sealed record InterruptionProjection(
        Guid Id,
        Guid TestAttemptId,
        DateTime InterruptedAt,
        string? Reason,
        Guid StatusId,
        string StatusBackendName,
        string StatusNameAr,
        string StatusNameEn,
        Guid? ResolutionActionId,
        string? ResolutionBackendName,
        string? ResolutionNameAr,
        string? ResolutionNameEn,
        string? ResolutionNotes,
        Guid? ResolvedById,
        DateTime? ResolvedAt);
}
