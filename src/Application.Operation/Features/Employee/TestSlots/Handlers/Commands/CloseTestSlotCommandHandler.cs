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
    ICurrentUserService currentUser)
    : IRequestHandler<CloseTestSlotCommand, IResult<Unit>>
{
    public Task<IResult<Unit>> Handle(CloseTestSlotCommand request, CancellationToken ct)
        => unitOfWork.ExecuteInTransactionAsync<IResult<Unit>>(async token =>
        {
            if (request.TestSlotId == Guid.Empty || !Guid.TryParse(currentUser.UserId, out var userId))
                return Result.Fail<Unit>(ErrorsCodes.InvalidRequest);

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

            testSlot.StatusId = TestSlotStatusIds.Closed;

            await unitOfWork.SaveChangesAsync(token);
            return Result.Ok(Unit.Value);
        }, ct);
}
