using System.Security.Cryptography;
using Application.Operation.Features.Employee.TestSlots.Commands;
using Application.Operation.Features.Employee.TestSlots.DTOs;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.TestSlots.Handlers.Commands;

public sealed class SaveTestSlotCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<SaveTestSlotCommand, IResult<SavedTestSlotDto>>
{
    public Task<IResult<SavedTestSlotDto>> Handle(SaveTestSlotCommand request, CancellationToken ct)
        => unitOfWork.ExecuteInTransactionAsync<IResult<SavedTestSlotDto>>(async token =>
        {
            var testSlot = request.TestSlot;
            var requestedStaff = testSlot.Staff ?? [];
            var titleAr = testSlot.TitleAr.Trim();
            var titleEn = string.IsNullOrWhiteSpace(testSlot.TitleEn) ? null : testSlot.TitleEn.Trim();
            var repository = unitOfWork.GetEntityRepository<TestSlot>();
            var slotId = request.Id ?? Guid.Empty;

            if (request.Id is { } id)
            {
                if (!await repository.DbSet.AnyAsync(x => x.Id == id, token))
                    return Result.Fail<SavedTestSlotDto>(ErrorsCodes.InvalidRequest);
            }

            if (!await unitOfWork.Context.Set<Room>().AnyAsync(
                    room => room.Id == testSlot.RoomId && room.StatusId == RoomStatusIds.Active, token))
                return Result.Fail<SavedTestSlotDto>(ErrorsCodes.TestSlotRoomNotAvailable);

            var staffIds = requestedStaff.Select(x => x.StaffUserId).Distinct().ToList();
            if (requestedStaff.Count == 0 || staffIds.Count != requestedStaff.Count ||
                requestedStaff.Any(staff => staff.StaffUserId == Guid.Empty || !staff.IsActive ||
                    (staff.RoleId != TestSlotStaffRoleIds.HallSupervisor &&
                     staff.RoleId != TestSlotStaffRoleIds.Monitor)) ||
                requestedStaff.Count(staff => staff.RoleId == TestSlotStaffRoleIds.HallSupervisor) != 1)
                return Result.Fail<SavedTestSlotDto>(ErrorsCodes.InvalidRequest);

            var testSlotStaffRoleId = await unitOfWork.Context.Set<ApplicationRole>()
                .Where(role => role.Name == nameof(SystemRoleIds.TestSlotStaffMember))
                .Select(role => (Guid?)role.Id)
                .SingleOrDefaultAsync(token);
            if (!testSlotStaffRoleId.HasValue ||
                await unitOfWork.Context.Set<User>().CountAsync(
                    user => staffIds.Contains(user.Id) && !user.IsDeleted && !user.IsBlocked &&
                            user.UserRoles.Any(userRole => userRole.RoleId == testSlotStaffRoleId.Value), token) !=
                staffIds.Count)
                return Result.Fail<SavedTestSlotDto>(ErrorsCodes.InvalidRequest);

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

            var roleIds = requestedStaff.Select(x => x.RoleId).Distinct().ToList();
            if (await unitOfWork.Context.Set<TestSlotStaffRole>().CountAsync(x => roleIds.Contains(x.Id), token) !=
                roleIds.Count)
                return Result.Fail<SavedTestSlotDto>(ErrorsCodes.TestSlotStaffRoleNotFound);

            var staffConflicts = await unitOfWork.Context.Set<TestSlotStaff>().AsNoTracking()
                .Where(x => x.TestSlotId != slotId && x.IsActive && staffIds.Contains(x.StaffUserId) &&
                            x.TestSlot!.SlotDate == testSlot.SlotDate && testSlot.StartTime < x.TestSlot.EndTime &&
                            testSlot.EndTime > x.TestSlot.StartTime)
                .Select(x => new TestSlotStaffConflictDto(x.StaffUserId, x.StaffUser!.FullNameAr,
                    x.TestSlot!.TitleAr, x.TestSlot.SlotDate, x.TestSlot.StartTime, x.TestSlot.EndTime))
                .ToListAsync(token);
            if (staffConflicts.Count != 0)
                return Result.Fail<SavedTestSlotDto>(new Error(ErrorsCodes.TestSlotStaffScheduleConflict)
                    .WithMetadata("Code", ErrorsCodes.TestSlotStaffScheduleConflict)
                    .WithMetadata("StatusCode", StatusCodes.Status409Conflict)
                    .WithMetadata("ConflictDetails", staffConflicts));

            var slot = request.Id is { } guid
                ? await repository.DbSet.FirstOrDefaultAsync(x => x.Id == guid, token)
                : new TestSlot
                {
                    AccessCodeHash = Convert.ToHexString(SHA256.HashData(RandomNumberGenerator.GetBytes(32))),
                    StatusId = TestSlotStatusIds.Ready,
                    TitleAr = string.Empty,
                };
            if (slot == null)
                return Result.Fail<SavedTestSlotDto>(ErrorsCodes.InvalidRequest);

            slot.TitleAr = titleAr;
            slot.TitleEn = titleEn;
            slot.RoomId = testSlot.RoomId;
            slot.SlotDate = testSlot.SlotDate;
            slot.StartTime = testSlot.StartTime;
            slot.EndTime = testSlot.EndTime;

            if (!request.Id.HasValue && (await repository.AddAsync(slot, token)).IsFailed)
                return Result.Fail<SavedTestSlotDto>(ErrorsCodes.InvalidRequest);

            var previousStaff = await unitOfWork.Context.Set<TestSlotStaff>()
                .Where(x => x.TestSlotId == slot.Id).ToListAsync(token);
            unitOfWork.RemoveRange(previousStaff);
            foreach (var staffDto in requestedStaff)
            {
                var staff = new TestSlotStaff
                {
                    TestSlotId = slot.Id,
                    StaffUserId = staffDto.StaffUserId,
                    RoleId = staffDto.RoleId,
                    IsActive = true
                };
                if ((await unitOfWork.GetEntityRepository<TestSlotStaff>().AddAsync(staff)).IsFailed)
                    return Result.Fail<SavedTestSlotDto>(ErrorsCodes.InvalidRequest);
            }

            await unitOfWork.SaveChangesAsync(token);
            return Result.Ok(new SavedTestSlotDto(slot.Id));
        }, ct);
}
