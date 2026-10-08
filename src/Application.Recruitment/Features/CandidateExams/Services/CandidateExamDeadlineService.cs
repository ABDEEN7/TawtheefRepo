using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Recruitment.Features.CandidateExams.Services;

public sealed record DueCandidateExamPart(
    Guid UserId,
    Guid AttemptId,
    Guid ActiveSessionId,
    Guid PartId);

public sealed class CandidateExamDeadlineService(
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    CandidateExamExecutionMutationService mutationService)
{
    public async Task<IReadOnlyList<DueCandidateExamPart>> GetDuePartsAsync(
        int take,
        CancellationToken cancellationToken)
    {
        if (take <= 0)
            return [];

        var now = timeProvider.GetUtcNow().UtcDateTime;

        return await unitOfWork.Context.Set<TestAttemptPart>()
            .AsNoTracking()
            .Where(part =>
                part.StatusId == TestAttemptPartStatusIds.InProgress &&
                part.EffectiveDeadlineAt != null &&
                part.EffectiveDeadlineAt <= now &&
                part.TestAttempt!.StatusId == TestAttemptStatusIds.InProgress &&
                part.TestAttempt.ActiveSessionId != null &&
                part.TestAttempt.TestSessionCandidate!.StatusId ==
                    TestSessionCandidateStatusIds.Started &&
                part.TestAttempt.TestSessionCandidate.TestSession!.StatusId ==
                    TestSessionStatusIds.InProgress &&
                part.TestAttempt.TestSessionCandidate.TestSession.TestSlot!.StatusId ==
                    TestSlotStatusIds.Started)
            .OrderBy(part => part.EffectiveDeadlineAt)
            .Select(part => new DueCandidateExamPart(
                part.TestAttempt!.TestSessionCandidate!.Invitation!.ApplicantId,
                part.TestAttemptId,
                part.TestAttempt.ActiveSessionId!.Value,
                part.Id))
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public Task<FluentResults.IResult<
        DTOs.TimeoutCandidateExamPartResponse>>
        ProcessAsync(
            DueCandidateExamPart duePart,
            CancellationToken cancellationToken) =>
        mutationService.TimeoutPartAsync(
            duePart.UserId,
            duePart.AttemptId,
            duePart.ActiveSessionId,
            duePart.PartId,
            cancellationToken);
}
