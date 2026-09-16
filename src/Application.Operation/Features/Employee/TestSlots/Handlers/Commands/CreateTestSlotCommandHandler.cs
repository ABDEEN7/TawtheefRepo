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

public sealed class CreateTestSlotCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateTestSlotCommand, IResult<CreatedTestSlotDto>>
{
    public Task<IResult<CreatedTestSlotDto>> Handle(CreateTestSlotCommand request, CancellationToken ct)
        => unitOfWork.ExecuteInTransactionAsync<IResult<CreatedTestSlotDto>>(async token =>
        {
            var testSlot = request.TestSlot;
            var titleAr = testSlot.TitleAr.Trim();
            var titleEn = string.IsNullOrWhiteSpace(testSlot.TitleEn) ? null : testSlot.TitleEn.Trim();
            var staffIds = testSlot.Staff.Select(x => x.StaffUserId).ToList();

            // Keep schedule checks and the subsequent insert serialized across concurrent create requests.
            await unitOfWork.Context.Database.ExecuteSqlRawAsync(
                "DECLARE @result int; EXEC @result = sys.sp_getapplock " +
                "@Resource = N'Tawtheef.TestSlot.Schedule', @LockMode = N'Exclusive', " +
                "@LockOwner = N'Transaction', @LockTimeout = 15000; " +
                "IF @result < 0 THROW 51000, 'Test slot schedule lock unavailable', 1;", token);

            var repository = unitOfWork.GetEntityRepository<TestSlot>();
            if (await repository.DbSet.AnyAsync(existingSlot => existingSlot.TitleAr == titleAr, token))
                return Result.Fail<CreatedTestSlotDto>(ErrorsCodes.TestSlotTitleArAlreadyExists);

            if (titleEn != null && await repository.DbSet.AnyAsync(existingSlot => existingSlot.TitleEn == titleEn, token))
                return Result.Fail<CreatedTestSlotDto>(ErrorsCodes.TestSlotTitleEnAlreadyExists);

            if (!await unitOfWork.Context.Set<Room>().AnyAsync(
                    room => room.Id == testSlot.RoomId && room.StatusId == RoomStatusIds.Active, token))
                return Result.Fail<CreatedTestSlotDto>(ErrorsCodes.TestSlotRoomNotAvailable);

            if (await repository.DbSet.AnyAsync(existingSlot =>
                    existingSlot.RoomId == testSlot.RoomId &&
                    existingSlot.SlotDate == testSlot.SlotDate &&
                    testSlot.StartTime < existingSlot.EndTime && testSlot.EndTime > existingSlot.StartTime, token))
                return Result.Fail<CreatedTestSlotDto>(ErrorsCodes.TestSlotRoomScheduleConflict);

            if (await unitOfWork.Context.Set<User>().CountAsync(x => staffIds.Contains(x.Id), token) != staffIds.Count)
                return Result.Fail<CreatedTestSlotDto>(ErrorsCodes.InvalidRequest);

            var roleIds = testSlot.Staff.Select(x => x.RoleId).Distinct().ToList();
            if (await unitOfWork.Context.Set<TestSlotStaffRole>().CountAsync(x => roleIds.Contains(x.Id), token) !=
                roleIds.Count)
                return Result.Fail<CreatedTestSlotDto>(ErrorsCodes.TestSlotStaffRoleNotFound);

            var staffConflicts = await unitOfWork.Context.Set<TestSlotStaff>().AsNoTracking()
                .Where(existingStaff =>
                    existingStaff.IsActive &&
                    staffIds.Contains(existingStaff.StaffUserId) &&
                    existingStaff.TestSlot!.SlotDate == testSlot.SlotDate &&
                    testSlot.StartTime < existingStaff.TestSlot.EndTime &&
                    testSlot.EndTime > existingStaff.TestSlot.StartTime)
                .Select(existingStaff => new TestSlotStaffConflictDto(
                    existingStaff.StaffUserId,
                    existingStaff.StaffUser!.FullNameAr,
                    existingStaff.TestSlot!.TitleAr,
                    existingStaff.TestSlot.SlotDate,
                    existingStaff.TestSlot.StartTime,
                    existingStaff.TestSlot.EndTime))
                .ToListAsync(token);
            if (staffConflicts.Count != 0)
                return Result.Fail<CreatedTestSlotDto>(new Error(ErrorsCodes.TestSlotStaffScheduleConflict)
                    .WithMetadata("Code", ErrorsCodes.TestSlotStaffScheduleConflict)
                    .WithMetadata("StatusCode", StatusCodes.Status409Conflict)
                    .WithMetadata("ConflictDetails", staffConflicts));

            var prefix = $"TS-{DateTime.UtcNow.Year}-";
            var numbers = await repository.DbSet.IgnoreQueryFilters().AsNoTracking()
                .Where(x => x.SlotNo.StartsWith(prefix)).Select(x => x.SlotNo).ToListAsync(token);
            var sequence = numbers.Select(x => long.TryParse(x[prefix.Length..], out var number) ? number : 0)
                .DefaultIfEmpty().Max() + 1;
            var slot = new TestSlot
            {
                SlotNo = $"{prefix}{sequence:D4}",
                TitleAr = titleAr,
                TitleEn = titleEn,
                RoomId = testSlot.RoomId,
                SlotDate = testSlot.SlotDate,
                StartTime = testSlot.StartTime,
                EndTime = testSlot.EndTime,
                AccessCodeHash = Convert.ToHexString(SHA256.HashData(RandomNumberGenerator.GetBytes(32))),
                StatusId = TestSlotStatusIds.Ready,
            };
            if ((await repository.AddAsync(slot)).IsFailed)
                return Result.Fail<CreatedTestSlotDto>(ErrorsCodes.InvalidRequest);

            foreach (var staffDto in testSlot.Staff)
            {
                var staff = new TestSlotStaff
                {
                    TestSlotId = slot.Id,
                    StaffUserId = staffDto.StaffUserId,
                    RoleId = staffDto.RoleId,
                    IsActive = true,
                };
                if ((await unitOfWork.GetEntityRepository<TestSlotStaff>().AddAsync(staff)).IsFailed)
                    return Result.Fail<CreatedTestSlotDto>(ErrorsCodes.InvalidRequest);
            }

            await unitOfWork.SaveChangesAsync(token);
            return Result.Ok(new CreatedTestSlotDto(slot.Id, slot.SlotNo));
        }, ct);
}
