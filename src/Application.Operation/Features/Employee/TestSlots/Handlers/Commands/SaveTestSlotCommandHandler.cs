using System.Globalization;
using System.Security.Cryptography;
using Application.Operation.Features.Employee.TestSlots.Commands;
using Application.Operation.Features.Employee.TestSlots.DTOs;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Employee.TestSlots.Handlers.Commands;

public sealed class SaveTestSlotCommandHandler(IUnitOfWork unitOfWork, IAccessCodeProtector accessCodeProtector)
    : IRequestHandler<SaveTestSlotCommand, IResult<SavedTestSlotDto>>
{
    public Task<IResult<SavedTestSlotDto>> Handle(SaveTestSlotCommand request, CancellationToken ct)
        => unitOfWork.ExecuteInTransactionAsync<IResult<SavedTestSlotDto>>(async token =>
        {
            var testSlot = request.TestSlot;
            var titleAr = testSlot.TitleAr.Trim();
            var titleEn = string.IsNullOrWhiteSpace(testSlot.TitleEn) ? null : testSlot.TitleEn.Trim();
            var repository = unitOfWork.GetEntityRepository<TestSlot>();
            var slotId = request.Id ?? Guid.Empty;

            if (request.Id is { } id && !await repository.DbSet.AnyAsync(x => x.Id == id, token))
                return Result.Fail<SavedTestSlotDto>(ErrorsCodes.InvalidRequest);

            if (!await unitOfWork.Context.Set<Room>().AnyAsync(
                    room => room.Id == testSlot.RoomId && room.StatusId == RoomStatusIds.Active, token))
                return Result.Fail<SavedTestSlotDto>(ErrorsCodes.TestSlotRoomNotAvailable);

            if (await repository.DbSet.AnyAsync(x => x.Id != slotId && x.TitleAr == titleAr, token))
                return Result.Fail<SavedTestSlotDto>(ErrorsCodes.TestSlotTitleArAlreadyExists);
            if (titleEn != null &&
                await repository.DbSet.AnyAsync(x => x.Id != slotId && x.TitleEn == titleEn, token))
                return Result.Fail<SavedTestSlotDto>(ErrorsCodes.TestSlotTitleEnAlreadyExists);
            if (await repository.DbSet.AnyAsync(x => x.Id != slotId && x.RoomId == testSlot.RoomId &&
                                                     x.SlotDate == testSlot.SlotDate &&
                                                     testSlot.StartTime < x.EndTime &&
                                                     testSlot.EndTime > x.StartTime, token))
                return Result.Fail<SavedTestSlotDto>(ErrorsCodes.TestSlotRoomScheduleConflict);

            var slot = request.Id is { } guid
                ? await repository.DbSet.FirstOrDefaultAsync(x => x.Id == guid, token)
                : CreateTestSlot();
            if (slot is null)
                return Result.Fail<SavedTestSlotDto>(ErrorsCodes.InvalidRequest);

            slot.TitleAr = titleAr;
            slot.TitleEn = titleEn;
            slot.RoomId = testSlot.RoomId;
            slot.SlotDate = testSlot.SlotDate;
            slot.StartTime = testSlot.StartTime;
            slot.EndTime = testSlot.EndTime;

            if (!request.Id.HasValue && (await repository.AddAsync(slot, token)).IsFailed)
                return Result.Fail<SavedTestSlotDto>(ErrorsCodes.InvalidRequest);

            var staffResult = await TestSlotAssignmentOperations.ReplaceAsync(
                unitOfWork, slot, testSlot.Staff ?? [], token);
            if (staffResult.IsFailed)
                return Result.Fail<SavedTestSlotDto>(staffResult.Errors);

            await unitOfWork.SaveChangesAsync(token);
            return Result.Ok(new SavedTestSlotDto(slot.Id));
        }, ct);

    private TestSlot CreateTestSlot()
    {
        var accessCode = RandomNumberGenerator.GetInt32(1000, 10000).ToString(CultureInfo.InvariantCulture);
        return new TestSlot
        {
            AccessCodeHash = accessCodeProtector.Protect(accessCode),
            StatusId = TestSlotStatusIds.Ready,
            TitleAr = string.Empty,
        };
    }
}
