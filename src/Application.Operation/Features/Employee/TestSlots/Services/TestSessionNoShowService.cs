using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Concurrency;
using Tawtheef.Application.Common.Interfaces.Concurrency;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Employee.TestSlots.Services;

public sealed record DueTestSessionNoShowCandidate(Guid CandidateId, Guid TestSlotId);

public sealed class TestSessionNoShowService(
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    IApplicationLockService applicationLockService)
{
    public async Task<IReadOnlyList<DueTestSessionNoShowCandidate>> GetDueAsync(int take, CancellationToken ct)
    {
        if (take <= 0)
            return [];

        var localNow = timeProvider.GetLocalNow().DateTime;

        var localDate = DateOnly.FromDateTime(localNow);

        var localTime = TimeOnly.FromDateTime(localNow);

        return await unitOfWork.Context.Set<TestSessionCandidate>()
            .AsNoTracking()
            .Where(candidate => candidate.StatusId == TestSessionCandidateStatusIds.Assigned
                                && candidate.AttendanceStatusId == TestSessionCandidateAttendanceStatusIds.Pending
                                && candidate.AuthorizedAt == null && candidate.TestAttempt == null
                                && candidate.TestSession != null && candidate.TestSession.EndTime != null
                                && (candidate.TestSession.StatusId == TestSessionStatusIds.Approved
                                    || candidate.TestSession.StatusId == TestSessionStatusIds.Ready
                                    || candidate.TestSession.StatusId == TestSessionStatusIds.InProgress)
                                && candidate.TestSession.TestSlot != null
                                && candidate.TestSession.TestSlot.StatusId == TestSlotStatusIds.Started
                                && (candidate.TestSession.TestSlot.SlotDate < localDate
                                    || (candidate.TestSession.TestSlot.SlotDate == localDate
                                        && candidate.TestSession.EndTime.Value <= localTime)))
            .OrderBy(candidate => candidate.TestSession!.TestSlot!.SlotDate)
            .ThenBy(candidate => candidate.TestSession!.EndTime)
            .Select(candidate =>
                new DueTestSessionNoShowCandidate(candidate.Id, candidate.TestSession!.TestSlotId!.Value))
            .Take(take)
            .ToListAsync(ct);
    }

    public Task<IResult<bool>> ProcessAsync(DueTestSessionNoShowCandidate dueCandidate, CancellationToken ct) =>
        unitOfWork.ExecuteInTransactionAsync<IResult<bool>>(async token =>
        {
            await applicationLockService.AcquireExclusiveAsync(
                ApplicationLockResources.CandidateExamTestSlot(dueCandidate.TestSlotId), token);

            var candidate = await unitOfWork.Context.Set<TestSessionCandidate>()
                .Include(x => x.TestAttempt)
                .Include(x => x.TestSession)
                .ThenInclude(x => x!.TestSlot)
                .SingleOrDefaultAsync(x => x.Id == dueCandidate.CandidateId, token);

            if (candidate?.TestSession?.TestSlot is not { } slot || candidate.TestSession.EndTime is null)
            {
                return Result.Ok(false);
            }

            var session = candidate.TestSession;

            if (slot.Id != dueCandidate.TestSlotId || slot.StatusId != TestSlotStatusIds.Started)
            {
                return Result.Ok(false);
            }

            if (session.StatusId != TestSessionStatusIds.Approved && session.StatusId != TestSessionStatusIds.Ready
                                                                  && session.StatusId
                                                                  != TestSessionStatusIds.InProgress)
            {
                return Result.Ok(false);
            }

            if (candidate.StatusId != TestSessionCandidateStatusIds.Assigned
                || candidate.AttendanceStatusId != TestSessionCandidateAttendanceStatusIds.Pending
                || candidate.AuthorizedAt is not null || candidate.TestAttempt is not null)
            {
                return Result.Ok(false);
            }

            var localNow = timeProvider.GetLocalNow().DateTime;

            var sessionEnd = slot.SlotDate.ToDateTime(session.EndTime.Value);

            if (localNow < sessionEnd)
            {
                return Result.Ok(false);
            }

            var markNoShowResult = candidate.MarkNoShow();

            if (markNoShowResult.IsFailed)
            {
                return Result.Fail<bool>(markNoShowResult.Errors);
            }

            await unitOfWork.SaveChangesAsync(token);

            return Result.Ok(true);
        }, ct);
}
