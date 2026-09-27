using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Configurations.Settings;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Recruitment.Features.CandidateExams.Services;

public sealed class CandidateExamInterruptionService(
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    IOptions<CandidateExamRealtimeSettings> options)
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
            .AnyAsync(x => x.Id == attemptId &&
                x.TestSessionCandidate!.Invitation!.ApplicantId == userId, cancellationToken);

        return isOwned && await TryInterruptCoreAsync(attemptId, userId, cancellationToken);
    }

    public async Task<IReadOnlyList<Guid>> GetDueAttemptIdsAsync(int take,
        CancellationToken cancellationToken)
    {
        if (take <= 0)
            return [];

        var now = timeProvider.GetUtcNow().UtcDateTime;
        var cutoff = now.AddSeconds(-_settings.InterruptionGraceSeconds);

        return await unitOfWork.Context.Set<TestAttempt>()
            .AsNoTracking()
            .Where(x => x.StatusId == TestAttemptStatusIds.InProgress &&
               x.Parts.Any(part =>
                   part.StatusId == TestAttemptPartStatusIds.InProgress &&
                   part.EffectiveDeadlineAt > now) &&
                x.Connections.Any() &&
                !x.Connections.Any(connection => connection.ExpiresAt > now) &&
                x.Connections.Max(connection => connection.ExpiresAt) <= cutoff)
            .OrderBy(x => x.Connections.Max(connection => connection.LastSeenAt))
            .Select(x => x.Id)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    private Task<bool> TryInterruptCoreAsync(Guid attemptId, Guid? userId,
        CancellationToken cancellationToken) =>
        unitOfWork.ExecuteInTransactionAsync(async token =>
        {
            await AcquireAttemptLockAsync(attemptId, token);

            var attempt = await unitOfWork.Context.Set<TestAttempt>()
                .Where(x => x.Id == attemptId &&
                    x.StatusId == TestAttemptStatusIds.InProgress &&
                    x.TestSessionCandidate!.StatusId == TestSessionCandidateStatusIds.Started &&
                    x.TestSessionCandidate.TestSession!.StatusId == TestSessionStatusIds.InProgress &&
                    x.TestSessionCandidate.TestSession.TestSlot!.StatusId == TestSlotStatusIds.Started &&
                    (!userId.HasValue || x.TestSessionCandidate.Invitation!.ApplicantId == userId.Value))
                .SingleOrDefaultAsync(token);
            if (attempt is null)
                return false;

            var activeParts = await unitOfWork.Context.Set<TestAttemptPart>()
                .Where(x => x.TestAttemptId == attemptId &&
                    x.StatusId == TestAttemptPartStatusIds.InProgress)
                .Take(2)
                .ToListAsync(token);
            if (activeParts.Count != 1)
                return false;

            var now = timeProvider.GetUtcNow().UtcDateTime;
            if (activeParts[0].EffectiveDeadlineAt is null ||
                activeParts[0].EffectiveDeadlineAt <= now)
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

            if (connectionState is null || connectionState.HasActiveConnection ||
                now < connectionState.LatestExpiresAt.AddSeconds(_settings.InterruptionGraceSeconds))
                return false;

            var hasOpenInterruption = await unitOfWork.Context.Set<TestAttemptInterruption>()
                .AsNoTracking()
                .AnyAsync(x => x.TestAttemptId == attemptId &&
                    x.StatusId == TestAttemptInterruptionStatusIds.Open, token);
            if (hasOpenInterruption)
                return false;

            attempt.StatusId = TestAttemptStatusIds.Interrupted;
            attempt.ActiveSessionId = null;
            activeParts[0].StatusId = TestAttemptPartStatusIds.Interrupted;
            await unitOfWork.Context.Set<TestAttemptInterruption>().AddAsync(
                new TestAttemptInterruption
                {
                    TestAttemptId = attemptId,
                    TestAttemptPartId = activeParts[0].Id,
                    InterruptedAt = connectionState.LatestLastSeenAt,
                    Reason = ConnectionLostReason,
                    StatusId = TestAttemptInterruptionStatusIds.Open
                }, token);

            await unitOfWork.SaveChangesAsync(token);

            await unitOfWork.Context.Set<CandidateExamConnection>()
                .Where(x => x.TestAttemptId == attemptId)
                .ExecuteDeleteAsync(token);

            return true;
        }, cancellationToken);

    private Task AcquireAttemptLockAsync(Guid attemptId, CancellationToken cancellationToken) =>
        unitOfWork.Context.Database.ExecuteSqlRawAsync(
            "DECLARE @result int; EXEC @result = sys.sp_getapplock " +
            "@Resource = {0}, @LockMode = N'Exclusive', @LockOwner = N'Transaction', " +
            "@LockTimeout = 15000; IF @result < 0 THROW 51000, 'Candidate exam interruption lock unavailable', 1;",
            [$"Tawtheef.CandidateExam.Attempt.{attemptId}"], cancellationToken);
}
