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

public sealed class CandidateExamConnectionService(
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    IOptions<CandidateExamRealtimeSettings> options,
    CandidateExamInterruptionService interruptionService,
    IApplicationLockService applicationLockService)
{
    private readonly CandidateExamRealtimeSettings _settings = options.Value;

    public async Task<bool> RegisterAsync(Guid userId, Guid attemptId, Guid activeSessionId, string connectionId,
        CancellationToken cancellationToken)
    {
        await interruptionService.TryInterruptForReconnectAsync(userId, attemptId, cancellationToken);

        if (!await IsRegistrationEligibleAsync(userId, attemptId, activeSessionId, cancellationToken))
            return false;

        return await unitOfWork.ExecuteInTransactionAsync(async token =>
        {
            await applicationLockService.AcquireExclusiveAsync(ApplicationLockResources.CandidateExamAttempt(attemptId),
                token);

            if (!await IsRegistrationEligibleAsync(userId, attemptId, activeSessionId, token))
                return false;

            var now = timeProvider.GetUtcNow().UtcDateTime;

            var connection = await unitOfWork.Context.Set<CandidateExamConnection>()
                .SingleOrDefaultAsync(x => x.ConnectionId == connectionId, token);

            if (connection is not null && connection.TestAttemptId != attemptId)
            {
                return false;
            }

            await unitOfWork.Context.Set<CandidateExamConnection>()
                .Where(x => x.TestAttemptId == attemptId && x.ConnectionId != connectionId)
                .ExecuteDeleteAsync(token);

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
            var attemptId = await unitOfWork.Context.Set<CandidateExamConnection>()
                .AsNoTracking()
                .Where(x => x.ConnectionId == connectionId)
                .Select(x => (Guid?)x.TestAttemptId)
                .SingleOrDefaultAsync(token);

            if (!attemptId.HasValue)
            {
                return false;
            }

            await applicationLockService.AcquireExclusiveAsync(
                ApplicationLockResources.CandidateExamAttempt(attemptId.Value), token);

            var now = timeProvider.GetUtcNow().UtcDateTime;

            var updatedRows = await unitOfWork.Context.Set<CandidateExamConnection>()
                .Where(x => x.ConnectionId == connectionId && x.TestAttemptId == attemptId.Value)
                .ExecuteUpdateAsync(
                    setters => setters.SetProperty(x => x.LastSeenAt, now).SetProperty(x => x.ExpiresAt, now), token);

            return updatedRows == 1;
        }, cancellationToken);
    }

    public async Task RenewAsync(IReadOnlyCollection<string> connectionIds, CancellationToken cancellationToken)
    {
        if (connectionIds.Count == 0)
            return;

        var now = timeProvider.GetUtcNow().UtcDateTime;
        var expiresAt = now.AddSeconds(_settings.ConnectionLeaseSeconds);
        await unitOfWork.Context.Set<CandidateExamConnection>()
            .Where(x => connectionIds.Contains(x.ConnectionId) && x.ExpiresAt > x.LastSeenAt)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(x => x.LastSeenAt, now).SetProperty(x => x.ExpiresAt, expiresAt),
                cancellationToken);
    }

    public Task<bool> HasActiveConnectionAsync(Guid attemptId, CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;
        return unitOfWork.Context.Set<CandidateExamConnection>()
            .AsNoTracking()
            .AnyAsync(x => x.TestAttemptId == attemptId && x.ExpiresAt > now, cancellationToken);
    }

    private Task<bool> IsRegistrationEligibleAsync(Guid userId, Guid attemptId, Guid activeSessionId,
        CancellationToken cancellationToken) =>
        unitOfWork.Context.Set<TestAttempt>()
            .AsNoTracking()
            .OwnedByApplicant(userId)
            .AnyAsync(
                x => x.Id == attemptId && x.StatusId == TestAttemptStatusIds.InProgress
                                       && x.ActiveSessionId == activeSessionId
                                       && x.TestSessionCandidate!.StatusId == TestSessionCandidateStatusIds.Started
                                       && x.TestSessionCandidate.TestSession!.StatusId
                                       == TestSessionStatusIds.InProgress
                                       && x.TestSessionCandidate.TestSession.TestSlot!.StatusId
                                       == TestSlotStatusIds.Started, cancellationToken);
}
