using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Tawtheef.Application.Common.Concurrency;
using Tawtheef.Application.Common.Interfaces.Concurrency;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Configurations.Settings;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Recruitment.Features.CandidateExams.Services;

public sealed class CandidateExamInterruptionService(
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    IOptions<CandidateExamRealtimeSettings> options,
    IApplicationLockService applicationLockService)
{
    private const string ConnectionLostReason = "Candidate realtime connection lost";
    private readonly CandidateExamRealtimeSettings _settings = options.Value;

    public Task<bool> TryInterruptAsync(Guid attemptId, CancellationToken cancellationToken) =>
        TryInterruptCoreAsync(attemptId, null, cancellationToken);

    public async Task<bool> TryInterruptForReconnectAsync(Guid userId, Guid attemptId,
        CancellationToken cancellationToken)
    {
        var isOwned = await unitOfWork.Context.Set<TestAttempt>()
            .AsNoTracking()
            .AnyAsync(x => x.Id == attemptId && x.TestSessionCandidate!.Invitation!.ApplicantId == userId,
                cancellationToken);

        return isOwned && await TryInterruptCoreAsync(attemptId, userId, cancellationToken);
    }

    public async Task<IReadOnlyList<Guid>> GetDueAttemptIdsAsync(int take, CancellationToken cancellationToken)
    {
        if (take <= 0)
            return [];

        var now = timeProvider.GetUtcNow().UtcDateTime;
        var cutoff = now.AddSeconds(-_settings.InterruptionGraceSeconds);

        var resolvedResumes = unitOfWork.Context.Set<TestAttemptInterruption>()
            .AsNoTracking()
            .Where(x => x.StatusId == TestAttemptInterruptionStatusIds.Resolved
                        && x.ResolutionActionId == TestAttemptInterruptionResolutionActionIds.Resume
                        && x.ResolvedAt.HasValue);

        return await (from attempt in unitOfWork.Context.Set<TestAttempt>().AsNoTracking()
                let latestResumeAt =
                    resolvedResumes.Where(interruption => interruption.TestAttemptId == attempt.Id)
                        .Max(interruption => interruption.ResolvedAt)
                where attempt.StatusId == TestAttemptStatusIds.InProgress
                      && attempt.Parts.Any(part =>
                          part.StatusId == TestAttemptPartStatusIds.InProgress && part.EffectiveDeadlineAt > now)
                      && ((attempt.Connections.Any()
                           && !attempt.Connections.Any(connection => connection.ExpiresAt > now)
                           && attempt.Connections.Max(connection => connection.ExpiresAt)
                           <= cutoff) || (!attempt.Connections.Any()
                                          && ((latestResumeAt.HasValue
                                               && latestResumeAt.Value <= cutoff)
                                              || (!latestResumeAt.HasValue
                                                  && attempt.StartedAt.HasValue
                                                  && attempt.StartedAt.Value <= cutoff))))
                orderby attempt.Connections.Any()
                    ? attempt.Connections.Max(connection => connection.LastSeenAt)
                    : latestResumeAt ?? attempt.StartedAt
                select attempt.Id).Take(take)
            .ToListAsync(cancellationToken);
    }

    private Task<bool> TryInterruptCoreAsync(Guid attemptId, Guid? userId, CancellationToken cancellationToken) =>
        unitOfWork.ExecuteInTransactionAsync(async token =>
        {
            await applicationLockService.AcquireExclusiveAsync(ApplicationLockResources.CandidateExamAttempt(attemptId),
                token);

            var attempt = await unitOfWork.Context.Set<TestAttempt>()
                .Where(x => x.Id == attemptId && x.StatusId == TestAttemptStatusIds.InProgress
                                              && x.TestSessionCandidate!.StatusId
                                              == TestSessionCandidateStatusIds.Started
                                              && x.TestSessionCandidate.TestSession!.StatusId
                                              == TestSessionStatusIds.InProgress
                                              && x.TestSessionCandidate.TestSession.TestSlot!.StatusId
                                              == TestSlotStatusIds.Started
                                              && (!userId.HasValue || x.TestSessionCandidate.Invitation!.ApplicantId
                                                  == userId.Value))
                .SingleOrDefaultAsync(token);

            if (attempt is null)
                return false;

            var activeParts = await unitOfWork.Context.Set<TestAttemptPart>()
                .Where(x => x.TestAttemptId == attemptId && x.StatusId == TestAttemptPartStatusIds.InProgress)
                .Take(2)
                .ToListAsync(token);

            if (activeParts.Count != 1)
                return false;

            var now = timeProvider.GetUtcNow().UtcDateTime;

            if (activeParts[0].EffectiveDeadlineAt is null || activeParts[0].EffectiveDeadlineAt <= now)
            {
                return false;
            }

            var connectionState = await unitOfWork.Context.Set<CandidateExamConnection>()
                .AsNoTracking()
                .Where(x => x.TestAttemptId == attemptId)
                .GroupBy(x => x.TestAttemptId)
                .Select(group => new
                {
                    LatestLastSeenAt = group.Max(x => x.LastSeenAt),
                    LatestExpiresAt = group.Max(x => x.ExpiresAt),
                    HasActiveConnection = group.Any(x => x.ExpiresAt > now)
                })
                .SingleOrDefaultAsync(token);

            DateTime interruptedAt;

            if (connectionState is null)
            {
                var latestResumeAt = await unitOfWork.Context.Set<TestAttemptInterruption>()
                    .AsNoTracking()
                    .Where(x => x.TestAttemptId == attemptId && x.StatusId == TestAttemptInterruptionStatusIds.Resolved
                                                             && x.ResolutionActionId
                                                             == TestAttemptInterruptionResolutionActionIds.Resume
                                                             && x.ResolvedAt.HasValue)
                    .MaxAsync(x => x.ResolvedAt, token);

                var disconnectedSince = latestResumeAt ?? attempt.StartedAt;

                if (disconnectedSince is not { } disconnectedAt
                    || now < disconnectedAt.AddSeconds(_settings.InterruptionGraceSeconds))
                {
                    return false;
                }

                interruptedAt = disconnectedAt;
            }
            else
            {
                if (connectionState.HasActiveConnection
                    || now < connectionState.LatestExpiresAt.AddSeconds(_settings.InterruptionGraceSeconds))
                {
                    return false;
                }

                interruptedAt = connectionState.LatestLastSeenAt;
            }

            var hasOpenInterruption = await unitOfWork.Context.Set<TestAttemptInterruption>()
                .AsNoTracking()
                .AnyAsync(x => x.TestAttemptId == attemptId && x.StatusId == TestAttemptInterruptionStatusIds.Open,
                    token);

            if (hasOpenInterruption)
                return false;

            attempt.Interrupt();
            activeParts[0].Interrupt();

            await unitOfWork.Context.Set<TestAttemptInterruption>()
                .AddAsync(
                    new TestAttemptInterruption
                    {
                        TestAttemptId = attemptId,
                        TestAttemptPartId = activeParts[0].Id,
                        InterruptedAt = interruptedAt,
                        Reason = ConnectionLostReason,
                        StatusId = TestAttemptInterruptionStatusIds.Open
                    }, token);

            await unitOfWork.SaveChangesAsync(token);

            await unitOfWork.Context.Set<CandidateExamConnection>()
                .Where(x => x.TestAttemptId == attemptId)
                .ExecuteDeleteAsync(token);

            return true;
        }, cancellationToken);
}
