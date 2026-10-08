using Application.Operation.Features.Employee.TestSlots.Commands;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Tawtheef.Application.Common.Concurrency;
using Tawtheef.Application.Common.Interfaces.Concurrency;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Configurations.Settings;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Employee.TestSlots.Handlers.Commands;

public sealed class StartTestSlotCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    TimeProvider timeProvider,
    IOptions<TestSlotSettings> settings,
    IApplicationLockService applicationLockService) : IRequestHandler<StartTestSlotCommand, IResult<Unit>>
{
    public Task<IResult<Unit>> Handle(StartTestSlotCommand request, CancellationToken ct) =>
        unitOfWork.ExecuteInTransactionAsync<IResult<Unit>>(async token =>
        {
            if (request.TestSlotId == Guid.Empty || !Guid.TryParse(currentUser.UserId, out var userId))
                return Result.Fail<Unit>(ErrorsCodes.InvalidRequest);

            await applicationLockService.AcquireExclusiveAsync(
                ApplicationLockResources.CandidateExamTestSlot(request.TestSlotId), token);

            var testSlot = await unitOfWork.GetEntityRepository<TestSlot>()
                .DbSet.FirstOrDefaultAsync(x => x.Id == request.TestSlotId, token);
            if (testSlot is null)
                return Result.Fail<Unit>(ErrorsCodes.ItemNotFound);
            if (testSlot.StatusId != TestSlotStatusIds.Ready)
                return Result.Fail<Unit>(ErrorsCodes.TestSlotCannotBeStartedInCurrentStatus);

            var isRoomHead = await unitOfWork.Context.Set<TestSlotStaff>()
                .AnyAsync(
                    staff => staff.TestSlotId == testSlot.Id && staff.StaffUserId == userId && staff.IsActive
                             && staff.RoleId == TestSlotStaffRoleIds.HallSupervisor, token);
            if (!isRoomHead)
                return Result.Fail<Unit>(
                    new Error(ErrorsCodes.UnauthorizedAction).WithMetadata("StatusCode",
                        StatusCodes.Status403Forbidden));

            var localNow = timeProvider.GetLocalNow().DateTime;
            if (testSlot.SlotDate != DateOnly.FromDateTime(localNow))
                return Result.Fail<Unit>(ErrorsCodes.TestSlotCanOnlyBeStartedOnScheduledDate);

            var earliestStart = testSlot.SlotDate.ToDateTime(testSlot.StartTime)
                .AddMinutes(-settings.Value.StartAllowedBeforeMinutes);
            if (localNow < earliestStart)
                return Result.Fail<Unit>(ErrorsCodes.TestSlotStartTooEarly);

            var scheduledEnd = testSlot.SlotDate.ToDateTime(testSlot.EndTime);

            if (localNow >= scheduledEnd)
            {
                return Result.Fail<Unit>(ErrorsCodes.TestSlotStartWindowClosed);
            }

            var sessions = await unitOfWork.Context.Set<TestSession>()
                .Where(x => x.TestSlotId == testSlot.Id && x.StatusId != TestSessionStatusIds.Cancelled)
                .ToListAsync(token);

            if (sessions.Count == 0 || sessions.Any(x =>
                    x.StatusId != TestSessionStatusIds.Approved && x.StatusId != TestSessionStatusIds.Ready))
            {
                return Result.Fail<Unit>(ErrorsCodes.TestSlotSessionStateInvalid);
            }

            var startedAt =
                timeProvider.GetUtcNow().UtcDateTime;

            testSlot.Start(
                userId,
                startedAt);

            await unitOfWork.SaveChangesAsync(token);
            return Result.Ok(Unit.Value);
        }, ct);
}
