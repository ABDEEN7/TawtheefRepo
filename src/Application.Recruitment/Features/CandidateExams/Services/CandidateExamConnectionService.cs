using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Configurations.Settings;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Recruitment.Features.CandidateExams.Services;

public sealed class CandidateExamConnectionService(
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    IOptions<CandidateExamRealtimeSettings> options,
    CandidateExamInterruptionService interruptionService)
{
    private readonly CandidateExamRealtimeSettings _settings = options.Value;

    public async Task<bool> RegisterAsync(Guid userId, Guid attemptId, Guid activeSessionId, string connectionId,
        CancellationToken cancellationToken)
    {
        await interruptionService.TryInterruptForReconnectAsync(userId, attemptId, cancellationToken);

        if (!await CanRegisterAsync(userId, attemptId, activeSessionId, cancellationToken))
            return false;

        return await unitOfWork.ExecuteInTransactionAsync(async token =>
        {
            await AcquireAttemptLockAsync(attemptId, token);

            if (!await CanRegisterAsync(userId, attemptId, activeSessionId, token))
                return false;

            var now = timeProvider.GetUtcNow().UtcDateTime;

            var connection = await unitOfWork.Context.Set<CandidateExamConnection>()
                .SingleOrDefaultAsync(x => x.ConnectionId == connectionId, token);
            
            await unitOfWork.Context.Set<CandidateExamConnection>()
                .Where(x =>
                    x.TestAttemptId == attemptId &&
                    x.ConnectionId != connectionId)
                .ExecuteDeleteAsync(token);

            if (connection is not null &&
                connection.TestAttemptId != attemptId)
            {
                return false;
            }

            if (connection is null)
            {
                connection = new CandidateExamConnection
                {
                    ConnectionId = connectionId,
                    TestAttemptId = attemptId,
                    LastSeenAt = now,
                    ExpiresAt = now.AddSeconds(_settings.ConnectionLeaseSeconds)
                };
                await unitOfWork.Context.Set<CandidateExamConnection>().AddAsync(connection, token);
            }
            else
            {
                connection.LastSeenAt = now;
                connection.ExpiresAt = now.AddSeconds(_settings.ConnectionLeaseSeconds);
            }


            await unitOfWork.SaveChangesAsync(token);
            return true;
        }, cancellationToken);
    }

    public async Task DisconnectAsync(string connectionId, CancellationToken cancellationToken)
    {
        await unitOfWork.ExecuteInTransactionAsync(async token =>
        {
            var connection = await unitOfWork.Context.Set<CandidateExamConnection>()
                .SingleOrDefaultAsync(x => x.ConnectionId == connectionId, token);
            if (connection is null)
                return false;

            await AcquireAttemptLockAsync(connection.TestAttemptId, token);

            var now = timeProvider.GetUtcNow().UtcDateTime;
            connection.LastSeenAt = now;
            connection.ExpiresAt = now;
            await unitOfWork.SaveChangesAsync(token);
            return true;
        }, cancellationToken);
    }

    public async Task RenewAsync(IReadOnlyCollection<string> connectionIds,
        CancellationToken cancellationToken)
    {
        if (connectionIds.Count == 0)
            return;

        var now = timeProvider.GetUtcNow().UtcDateTime;
        var expiresAt = now.AddSeconds(_settings.ConnectionLeaseSeconds);
        await unitOfWork.Context.Set<CandidateExamConnection>()
            .Where(x => connectionIds.Contains(x.ConnectionId) &&
                        x.ExpiresAt > x.LastSeenAt)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.LastSeenAt, now)
                .SetProperty(x => x.ExpiresAt, expiresAt), cancellationToken);
    }

    public Task<bool> HasActiveConnectionAsync(Guid attemptId, CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;
        return unitOfWork.Context.Set<CandidateExamConnection>()
            .AsNoTracking()
            .AnyAsync(x => x.TestAttemptId == attemptId && x.ExpiresAt > now, cancellationToken);
    }

    private Task<bool> CanRegisterAsync(
        Guid userId,
        Guid attemptId,
        Guid activeSessionId,
        CancellationToken cancellationToken) =>
        unitOfWork.Context.Set<TestAttempt>()
            .AsNoTracking()
            .AnyAsync(x =>
                    x.Id == attemptId &&
                    x.TestSessionCandidate!.Invitation!.ApplicantId == userId &&
                    x.StatusId == TestAttemptStatusIds.InProgress &&
                    x.ActiveSessionId == activeSessionId &&
                    x.TestSessionCandidate.StatusId ==
                    TestSessionCandidateStatusIds.Started &&
                    x.TestSessionCandidate.TestSession!.StatusId ==
                    TestSessionStatusIds.InProgress &&
                    x.TestSessionCandidate.TestSession.TestSlot!.StatusId ==
                    TestSlotStatusIds.Started,
                cancellationToken);

    private Task AcquireAttemptLockAsync(Guid attemptId, CancellationToken cancellationToken) =>
        unitOfWork.Context.Database.ExecuteSqlRawAsync(
            "DECLARE @result int; EXEC @result = sys.sp_getapplock " +
            "@Resource = {0}, @LockMode = N'Exclusive', @LockOwner = N'Transaction', " +
            "@LockTimeout = 15000; IF @result < 0 THROW 51000, 'Candidate exam connection lock unavailable', 1;",
            [$"Tawtheef.CandidateExam.Attempt.{attemptId}"], cancellationToken);
}
