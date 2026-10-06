using Application.Recruitment.Features.CandidateExams.Common;
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
            .OwnedByApplicant(userId)
            .AnyAsync(x => x.Id == attemptId, cancellationToken);

        return isOwned && await TryInterruptCoreAsync(attemptId, userId, cancellationToken);
    }

    private IQueryable<ConnectionState> QueryConnectionStates() =>
        unitOfWork.Context.Set<CandidateExamConnection>()
            .AsNoTracking()
            .GroupBy(connection => connection.TestAttemptId)
            .Select(group => new ConnectionState
            {
                TestAttemptId = group.Key,
                LatestLastSeenAt = group.Max(x => x.LastSeenAt),
                LatestExpiresAt = group.Max(x => x.ExpiresAt)
            });

    private IQueryable<ResolvedResumeState> QueryResolvedResumeStates() =>
        unitOfWork.Context.Set<TestAttemptInterruption>()
            .AsNoTracking()
            .Where(interruption => interruption.StatusId == TestAttemptInterruptionStatusIds.Resolved
                                   && interruption.ResolutionActionId
                                   == TestAttemptInterruptionResolutionActionIds.Resume
                                   && interruption.ResolvedAt.HasValue)
            .GroupBy(interruption => interruption.TestAttemptId)
            .Select(group => new ResolvedResumeState
            {
                TestAttemptId = group.Key, LatestResumeAt = group.Max(x => x.ResolvedAt)
            });

    public async Task<IReadOnlyList<Guid>> GetDueAttemptIdsAsync(int take, CancellationToken cancellationToken)
    {
        if (take <= 0)
            return [];

        var now = timeProvider.GetUtcNow().UtcDateTime;

        var cutoff = now.AddSeconds(-_settings.InterruptionGraceSeconds);

        var query = unitOfWork.Context.Set<TestAttempt>()
            .AsNoTracking()
            .Where(attempt => attempt.StatusId == TestAttemptStatusIds.InProgress
                              && attempt.Parts.Any(part =>
                                  part.StatusId == TestAttemptPartStatusIds.InProgress
                                  && part.EffectiveDeadlineAt > now))
            .Select(attempt => new
            {
                attempt.Id,
                attempt.StartedAt,
                LatestLastSeenAt = unitOfWork.Context.Set<CandidateExamConnection>()
                    .Where(connection => connection.TestAttemptId == attempt.Id)
                    .Select(connection => (DateTime?)connection.LastSeenAt)
                    .Max(),
                LatestExpiresAt = unitOfWork.Context.Set<CandidateExamConnection>()
                    .Where(connection => connection.TestAttemptId == attempt.Id)
                    .Select(connection => (DateTime?)connection.ExpiresAt)
                    .Max(),
                LatestResumeAt = unitOfWork.Context.Set<TestAttemptInterruption>()
                    .Where(interruption => interruption.TestAttemptId == attempt.Id
                                           && interruption.StatusId
                                           == TestAttemptInterruptionStatusIds.Resolved
                                           && interruption.ResolutionActionId
                                           == TestAttemptInterruptionResolutionActionIds.Resume
                                           && interruption.ResolvedAt.HasValue)
                    .Select(interruption => interruption.ResolvedAt)
                    .Max()
            })
            .Where(state => state.LatestExpiresAt.HasValue
                ? state.LatestExpiresAt.Value <= cutoff
                : state.LatestResumeAt.HasValue
                    ? state.LatestResumeAt.Value <= cutoff
                    : state.StartedAt.HasValue && state.StartedAt.Value <= cutoff)
            .OrderBy(state => state.LatestExpiresAt.HasValue
                ? state.LatestLastSeenAt
                : state.LatestResumeAt ?? state.StartedAt)
            .Select(state => state.Id);

        return await query.Take(take).ToListAsync(cancellationToken);
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

            var connectionState = await QueryConnectionStates()
                .SingleOrDefaultAsync(x => x.TestAttemptId == attemptId, token);

            DateTime interruptedAt;

            if (connectionState is null)
            {
                var latestResumeAt = await QueryResolvedResumeStates()
                    .Where(x => x.TestAttemptId == attemptId)
                    .Select(x => x.LatestResumeAt)
                    .SingleOrDefaultAsync(token);

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
                if (now < connectionState.LatestExpiresAt.AddSeconds(_settings.InterruptionGraceSeconds))
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

    private sealed class ConnectionState
    {
        public Guid TestAttemptId { get; init; }
        public DateTime LatestLastSeenAt { get; init; }
        public DateTime LatestExpiresAt { get; init; }
    }

    private sealed class ResolvedResumeState
    {
        public Guid TestAttemptId { get; init; }
        public DateTime? LatestResumeAt { get; init; }
    }
}
