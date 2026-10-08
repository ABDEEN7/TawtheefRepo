using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Concurrency;
using Tawtheef.Application.Common.Interfaces.Concurrency;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Employee.TestSlots.Services;

public sealed class TestSlotCandidateHallActionService(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    TimeProvider timeProvider,
    IApplicationLockService applicationLockService)
{
    public async Task<IResult<TestSessionCandidate>> LoadCandidateForUpdateAsync(Guid testSlotId, Guid candidateId,
        CancellationToken cancellationToken)
    {
        if (testSlotId == Guid.Empty || candidateId == Guid.Empty || !Guid.TryParse(currentUser.UserId, out var userId))
        {
            return Result.Fail<TestSessionCandidate>(ErrorsCodes.InvalidRequest);
        }

        await applicationLockService.AcquireExclusiveAsync(ApplicationLockResources.CandidateExamTestSlot(testSlotId),
            cancellationToken);

        var slot = await unitOfWork.Context.Set<TestSlot>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == testSlotId, cancellationToken);

        if (slot is null)
        {
            return Result.Fail<TestSessionCandidate>(ErrorsCodes.ItemNotFound);
        }

        if (slot.StatusId != TestSlotStatusIds.Started)
        {
            return Result.Fail<TestSessionCandidate>(ErrorsCodes.CandidateExamSlotNotRunning);
        }

        var isHallSupervisor = await unitOfWork.Context.Set<TestSlotStaff>()
            .AnyAsync(x => x.TestSlotId == testSlotId &&
                           x.StaffUserId == userId &&
                           x.IsActive &&
                           x.RoleId == TestSlotStaffRoleIds.HallSupervisor
                , cancellationToken);

        if (!isHallSupervisor)
        {
            return Result.Fail<TestSessionCandidate>(
                new Error(ErrorsCodes.UnauthorizedAction).WithMetadata("StatusCode", StatusCodes.Status403Forbidden));
        }

        var candidate = await unitOfWork.Context.Set<TestSessionCandidate>()
            .Include(x => x.TestSession)
            .FirstOrDefaultAsync(x => x.Id == candidateId && x.TestSession!.TestSlotId == testSlotId,
                cancellationToken);

        if (candidate is null)
        {
            return Result.Fail<TestSessionCandidate>(ErrorsCodes.ItemNotFound);
        }

        var session = candidate.TestSession;

        if (session?.EndTime is null)
        {
            return Result.Fail<TestSessionCandidate>(ErrorsCodes.TestSlotCandidateHallStateInvalid);
        }

        var localNow = timeProvider.GetLocalNow().DateTime;

        var sessionEnd = slot.SlotDate.ToDateTime(session.EndTime.Value);

        if (localNow >= sessionEnd)
        {
            return Result.Fail<TestSessionCandidate>(ErrorsCodes.TestSlotCandidateHallStateInvalid);
        }

        if (!IsHallActionAllowedSessionStatus(session.StatusId))
        {
            return Result.Fail<TestSessionCandidate>(ErrorsCodes.CandidateExamSessionNotRunning);
        }

        if (IsHallActionBlockedCandidateStatus(candidate.StatusId))
        {
            return Result.Fail<TestSessionCandidate>(ErrorsCodes.TestSlotCandidateHallStateInvalid);
        }

        return Result.Ok(candidate);
    }

    private static bool IsHallActionAllowedSessionStatus(Guid statusId) =>
        statusId == TestSessionStatusIds.Approved || statusId == TestSessionStatusIds.Ready
                                                  || statusId == TestSessionStatusIds.InProgress;

    private static bool IsHallActionBlockedCandidateStatus(Guid statusId) =>
        statusId == TestSessionCandidateStatusIds.Cancelled || statusId == TestSessionCandidateStatusIds.Rescheduled
                                                            || statusId == TestSessionCandidateStatusIds.Completed;
}
