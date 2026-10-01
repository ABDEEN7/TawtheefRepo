using Application.Operation.Features.Employee.TestSlots.Commands;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Employee.TestSlots.Handlers.Commands;

public sealed class CloseTestSlotCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    TimeProvider timeProvider)
    : IRequestHandler<CloseTestSlotCommand, IResult<Unit>>
{
    public Task<IResult<Unit>> Handle(CloseTestSlotCommand request, CancellationToken ct)
        => unitOfWork.ExecuteInTransactionAsync<IResult<Unit>>(async token =>
        {
            if (request.TestSlotId == Guid.Empty || !Guid.TryParse(currentUser.UserId, out var userId))
                return Result.Fail<Unit>(ErrorsCodes.InvalidRequest);

            await TestSlotCandidateHallState.LockAsync(unitOfWork, request.TestSlotId, token);

            var testSlot = await unitOfWork.GetEntityRepository<TestSlot>().DbSet
                .FirstOrDefaultAsync(x => x.Id == request.TestSlotId, token);
            if (testSlot is null)
                return Result.Fail<Unit>(ErrorsCodes.ItemNotFound);
            if (testSlot.StatusId != TestSlotStatusIds.Started)
                return Result.Fail<Unit>(ErrorsCodes.TestSlotCannotBeClosedInCurrentStatus);

            var isRoomHead = await unitOfWork.Context.Set<TestSlotStaff>().AnyAsync(staff =>
                staff.TestSlotId == testSlot.Id && staff.StaffUserId == userId && staff.IsActive &&
                staff.RoleId == TestSlotStaffRoleIds.HallSupervisor, token);
            if (!isRoomHead)
                return Result.Fail<Unit>(new Error(ErrorsCodes.UnauthorizedAction)
                    .WithMetadata("StatusCode", StatusCodes.Status403Forbidden));

            var sessions = await unitOfWork.Context
                .Set<TestSession>()
                .Where(x =>
                    x.TestSlotId == testSlot.Id &&
                    x.StatusId != TestSessionStatusIds.Cancelled)
                .ToListAsync(token);

            if (sessions.Count == 0 ||
                sessions.Any(x =>
                    x.StatusId !=
                    TestSessionStatusIds.Approved &&
                    x.StatusId !=
                    TestSessionStatusIds.Ready &&
                    x.StatusId !=
                    TestSessionStatusIds.InProgress &&
                    x.StatusId !=
                    TestSessionStatusIds.Closed))
            {
                return Result.Fail<Unit>(
                    ErrorsCodes.TestSlotSessionStateInvalid);
            }

            var sessionIds = sessions
                .Select(x => x.Id)
                .ToList();

            var hasActiveAttempts = await unitOfWork.Context
                .Set<TestAttempt>()
                .AsNoTracking()
                .AnyAsync(x =>
                        sessionIds.Contains(
                            x.TestSessionCandidate!.TestSessionId) &&
                        x.StatusId == TestAttemptStatusIds.InProgress,
                    token);

            if (hasActiveAttempts)
            {
                return Result.Fail<Unit>(
                    ErrorsCodes.TestSlotHasActiveExamAttempts);
            }

            var localNow = timeProvider.GetLocalNow().DateTime;

            var scheduledEnd =
                testSlot.SlotDate.ToDateTime(testSlot.EndTime);

            var slotHasEnded =
                localNow >= scheduledEnd;

            if (!slotHasEnded)
            {
                var hasUnfinishedCandidates =
                    await unitOfWork.Context
                        .Set<TestSessionCandidate>()
                        .AsNoTracking()
                        .AnyAsync(x =>
                                sessionIds.Contains(x.TestSessionId) &&
                                x.StatusId !=
                                TestSessionCandidateStatusIds.Completed &&
                                x.StatusId !=
                                TestSessionCandidateStatusIds.Cancelled &&
                                x.StatusId !=
                                TestSessionCandidateStatusIds.Rescheduled,
                            token);

                if (hasUnfinishedCandidates)
                {
                    return Result.Fail<Unit>(
                        ErrorsCodes.TestSlotCannotBeClosedBeforeEnd);
                }
            }

            if (slotHasEnded)
            {
                var noShows = await unitOfWork.Context
                    .Set<TestSessionCandidate>()
                    .Where(x =>
                        sessionIds.Contains(x.TestSessionId) &&
                        x.StatusId ==
                        TestSessionCandidateStatusIds.Assigned &&
                        x.AttendanceStatusId ==
                        TestSessionCandidateAttendanceStatusIds.Pending &&
                        x.TestAttempt == null)
                    .ToListAsync(token);

                foreach (var candidate in noShows)
                {
                    candidate.AttendanceStatusId = TestSessionCandidateAttendanceStatusIds.NoShow;
                }
            }

            foreach (var session in sessions.Where( session => session.StatusId != TestSessionStatusIds.Closed))
            {
                session.StatusId = TestSessionStatusIds.Closed;
            }

            testSlot.StatusId =
                TestSlotStatusIds.Closed;

            await unitOfWork.SaveChangesAsync(token);

            return Result.Ok(Unit.Value);
        }, ct);
}
