using Application.Operation.Features.Employee.TestSessions.DTOs;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Employee.TestSessions.Services;

public static class TestSessionCandidateConflictService
{
    public static readonly Guid[] NonBlockingStatusIds =
    [
        TestSessionStatusIds.Draft,
        TestSessionStatusIds.Cancelled,
        TestSessionStatusIds.Rejected
    ];

    public static IQueryable<TestSessionCandidate> BlockingAssignments(
        ITawtheefDbContext context,
        Guid jobId,
        Guid currentTestSessionId) =>
        context.Set<TestSessionCandidate>().AsNoTracking().Where(candidate =>
            candidate.TestSessionId != currentTestSessionId &&
            candidate.TestSession!.Exam!.JobId == jobId &&
            !NonBlockingStatusIds.Contains(candidate.TestSession.StatusId));

    public static async Task<IReadOnlyList<TestSessionCandidateConflictDto>> GetConflictsAsync(
        ITawtheefDbContext context,
        Guid jobId,
        Guid currentTestSessionId,
        IReadOnlyCollection<Guid> invitationIds,
        bool isArabic,
        CancellationToken ct)
    {
        if (invitationIds.Count == 0)
            return [];

        return await BlockingAssignments(context, jobId, currentTestSessionId)
            .Where(candidate => invitationIds.Contains(candidate.InvitationId))
            .Select(candidate => new TestSessionCandidateConflictDto(
                candidate.InvitationId,
                isArabic
                    ? candidate.Invitation!.Applicant!.FullNameAr
                    : candidate.Invitation!.Applicant!.FullNameEn,
                candidate.Invitation!.Applicant!.Profile == null
                    ? null
                    : candidate.Invitation.Applicant.Profile.NationalNumber))
            .Distinct()
            .ToListAsync(ct);
    }
}
