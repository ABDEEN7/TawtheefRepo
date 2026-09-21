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
using Tawtheef.Domain.Events.Operation.Employee.TestSlots;

namespace Application.Operation.Features.Employee.TestSlots.Handlers.Commands;

internal static class TestSlotAssignmentOperations
{
    public static async Task<IResult<Unit>> ReplaceAsync(
        IUnitOfWork unitOfWork,
        TestSlot testSlot,
        IReadOnlyCollection<TestSlotStaffAssignmentDto> requestedStaff,
        CancellationToken token)
    {
        var staffIds = requestedStaff.Select(x => x.StaffUserId).Distinct().ToList();
        if (requestedStaff.Count == 0 || staffIds.Count != requestedStaff.Count ||
            requestedStaff.Any(staff => staff.StaffUserId == Guid.Empty || !staff.IsActive ||
                (staff.RoleId != TestSlotStaffRoleIds.HallSupervisor &&
                 staff.RoleId != TestSlotStaffRoleIds.Monitor)) ||
            requestedStaff.Count(staff => staff.RoleId == TestSlotStaffRoleIds.HallSupervisor) != 1)
            return Result.Fail<Unit>(ErrorsCodes.InvalidRequest);

        var testSlotStaffRoleId = await unitOfWork.Context.Set<ApplicationRole>()
            .Where(role => role.Name == nameof(SystemRoleIds.TestSlotStaffMember))
            .Select(role => (Guid?)role.Id)
            .SingleOrDefaultAsync(token);
        if (!testSlotStaffRoleId.HasValue ||
            await unitOfWork.Context.Set<User>().CountAsync(
                user => staffIds.Contains(user.Id) && !user.IsDeleted && !user.IsBlocked &&
                        user.UserRoles.Any(userRole => userRole.RoleId == testSlotStaffRoleId.Value), token) !=
            staffIds.Count)
            return Result.Fail<Unit>(ErrorsCodes.InvalidRequest);

        var roleIds = requestedStaff.Select(x => x.RoleId).Distinct().ToList();
        if (await unitOfWork.Context.Set<TestSlotStaffRole>().CountAsync(x => roleIds.Contains(x.Id), token) !=
            roleIds.Count)
            return Result.Fail<Unit>(ErrorsCodes.TestSlotStaffRoleNotFound);

        var staffConflicts = await unitOfWork.Context.Set<TestSlotStaff>().AsNoTracking()
            .Where(x => x.TestSlotId != testSlot.Id && x.IsActive && staffIds.Contains(x.StaffUserId) &&
                        x.TestSlot!.SlotDate == testSlot.SlotDate && testSlot.StartTime < x.TestSlot.EndTime &&
                        testSlot.EndTime > x.TestSlot.StartTime)
            .Select(x => new TestSlotStaffConflictDto(x.StaffUserId, x.StaffUser!.FullNameAr,
                x.TestSlot!.TitleAr, x.TestSlot.SlotDate, x.TestSlot.StartTime, x.TestSlot.EndTime))
            .ToListAsync(token);
        if (staffConflicts.Count != 0)
            return Result.Fail<Unit>(new Error(ErrorsCodes.TestSlotStaffScheduleConflict)
                .WithMetadata("Code", ErrorsCodes.TestSlotStaffScheduleConflict)
                .WithMetadata("StatusCode", StatusCodes.Status409Conflict)
                .WithMetadata("ConflictDetails", staffConflicts));

        var existingStaff = await unitOfWork.Context.Set<TestSlotStaff>()
            .Where(x => x.TestSlotId == testSlot.Id).ToListAsync(token);
        var previousActiveStaff = existingStaff.Where(staff => staff.IsActive).ToList();
        unitOfWork.RemoveRange(existingStaff);
        foreach (var staffDto in requestedStaff)
        {
            var staff = new TestSlotStaff
            {
                TestSlotId = testSlot.Id,
                StaffUserId = staffDto.StaffUserId,
                RoleId = staffDto.RoleId,
                IsActive = true,
            };
            if ((await unitOfWork.GetEntityRepository<TestSlotStaff>().AddAsync(staff, token)).IsFailed)
                return Result.Fail<Unit>(ErrorsCodes.InvalidRequest);
        }

        AddAssignmentChangeEvents(testSlot, previousActiveStaff, requestedStaff);

        return Result.Ok(Unit.Value);
    }

    private static void AddAssignmentChangeEvents(
        TestSlot testSlot,
        IReadOnlyCollection<TestSlotStaff> previousStaff,
        IReadOnlyCollection<TestSlotStaffAssignmentDto> requestedStaff)
    {
        var previousAssignments = previousStaff
            .Select(staff => (staff.StaffUserId, staff.RoleId))
            .Distinct()
            .ToHashSet();
        var requestedAssignments = requestedStaff
            .Select(staff => (staff.StaffUserId, staff.RoleId))
            .Distinct()
            .ToHashSet();

        var changes = previousAssignments.Except(requestedAssignments)
            .Select(assignment => new TestSlotStaffAssignmentNotification(
                assignment.StaffUserId,
                assignment.RoleId,
                TestSlotStaffAssignmentNotificationType.Unassigned))
            .Concat(requestedAssignments.Except(previousAssignments)
                .Select(assignment => new TestSlotStaffAssignmentNotification(
                    assignment.StaffUserId,
                    assignment.RoleId,
                    TestSlotStaffAssignmentNotificationType.Assigned)))
            .ToList();

        if (changes.Count != 0)
        {
            testSlot.AddDomainEvent(new TestSlotStaffAssignmentsChangedDomainEvent(
                testSlot.Id, testSlot.TitleAr, testSlot.TitleEn, testSlot.SlotDate, changes));
        }
    }
}
