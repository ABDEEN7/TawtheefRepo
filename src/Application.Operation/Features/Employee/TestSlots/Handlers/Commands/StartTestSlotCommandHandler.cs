using Application.Operation.Features.Employee.TestSlots.Commands;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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
    IOptions<TestSlotSettings> settings)
    : IRequestHandler<StartTestSlotCommand, IResult<Unit>>
{
    public Task<IResult<Unit>> Handle(StartTestSlotCommand request, CancellationToken ct)
        => unitOfWork.ExecuteInTransactionAsync<IResult<Unit>>(async token =>
        {
            if (request.TestSlotId == Guid.Empty || !Guid.TryParse(currentUser.UserId, out var userId))
                return Result.Fail<Unit>(ErrorsCodes.InvalidRequest);

            var testSlot = await unitOfWork.GetEntityRepository<TestSlot>().DbSet
                .FirstOrDefaultAsync(x => x.Id == request.TestSlotId, token);
            if (testSlot is null)
                return Result.Fail<Unit>(ErrorsCodes.ItemNotFound);
            if (testSlot.StatusId != TestSlotStatusIds.Ready)
                return Result.Fail<Unit>(ErrorsCodes.TestSlotCannotBeStartedInCurrentStatus);

            var isRoomHead = await unitOfWork.Context.Set<TestSlotStaff>().AnyAsync(staff =>
                staff.TestSlotId == testSlot.Id && staff.StaffUserId == userId && staff.IsActive &&
                staff.RoleId == TestSlotStaffRoleIds.HallSupervisor, token);
            if (!isRoomHead)
                return Result.Fail<Unit>(new Error(ErrorsCodes.UnauthorizedAction)
                    .WithMetadata("StatusCode", StatusCodes.Status403Forbidden));

            var localNow = timeProvider.GetLocalNow().DateTime;
            if (testSlot.SlotDate != DateOnly.FromDateTime(localNow))
                return Result.Fail<Unit>(ErrorsCodes.TestSlotCanOnlyBeStartedOnScheduledDate);

            var earliestStart = testSlot.SlotDate.ToDateTime(testSlot.StartTime)
                .AddMinutes(-settings.Value.StartAllowedBeforeMinutes);
            if (localNow < earliestStart)
                return Result.Fail<Unit>(ErrorsCodes.TestSlotStartTooEarly);

            testSlot.StatusId = TestSlotStatusIds.Started;
            testSlot.StartedById = userId;
            testSlot.StartedAt = timeProvider.GetUtcNow().UtcDateTime;

            await unitOfWork.SaveChangesAsync(token);
            return Result.Ok(Unit.Value);
        }, ct);
}
