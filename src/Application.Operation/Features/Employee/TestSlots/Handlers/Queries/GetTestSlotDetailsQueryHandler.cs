using Application.Operation.Features.Employee.TestSlots.DTOs;
using Application.Operation.Features.Employee.TestSlots.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Application.Common.Models;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Employee.TestSlots.Handlers.Queries;

public sealed class GetTestSlotDetailsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    : IRequestHandler<GetTestSlotDetailsQuery, IResult<TestSlotDetailsDto>>
{
    public async Task<IResult<TestSlotDetailsDto>> Handle(GetTestSlotDetailsQuery request, CancellationToken ct)
    {
        var isArabic = string.Equals(request.Language, "ar", StringComparison.OrdinalIgnoreCase);
        var currentUserId = Guid.TryParse(currentUser.UserId, out var id) ? id : Guid.Empty;
        var slot = await unitOfWork.Context.Set<TestSlot>().AsNoTracking()
            .Where(x => x.Id == request.TestSlotId)
            .Select(x => new TestSlotDetailsDto
            {
                Id = x.Id,
                TitleAr = x.TitleAr,
                TitleEn = x.TitleEn,
                RoomName = isArabic ? x.Room!.NameAr : x.Room!.NameEn ?? x.Room.NameAr,
                RoomCapacity = x.Room!.Capacity,
                SlotDate = x.SlotDate,
                StartTime = x.StartTime,
                EndTime = x.EndTime,
                Status = new DropdownOptions { Id = x.StatusId, BackendName = x.Status!.BackendName,
                    Name = isArabic ? x.Status.NameAr : x.Status.NameEn },
                CandidateCount = unitOfWork.Context.Set<TestSessionCandidate>().Count(c =>
                    c.TestSession!.TestSlotId == x.Id),
                TotalCandidates = unitOfWork.Context.Set<TestSessionCandidate>().Count(c =>
                    c.TestSession!.TestSlotId == x.Id),
                PresentCandidates = unitOfWork.Context.Set<TestSessionCandidate>().Count(c =>
                    c.TestSession!.TestSlotId == x.Id &&
                    c.AttendanceStatusId == TestSessionCandidateAttendanceStatusIds.Present),
                WaitingCandidates = unitOfWork.Context.Set<TestSessionCandidate>().Count(c =>
                    c.TestSession!.TestSlotId == x.Id &&
                    c.AttendanceStatusId == TestSessionCandidateAttendanceStatusIds.Pending),
                AbsentCandidates = unitOfWork.Context.Set<TestSessionCandidate>().Count(c =>
                    c.TestSession!.TestSlotId == x.Id &&
                    c.AttendanceStatusId == TestSessionCandidateAttendanceStatusIds.NoShow),
                Staff = unitOfWork.Context.Set<TestSlotStaff>().Where(s => s.TestSlotId == x.Id && s.IsActive)
                    .OrderBy(s => s.Id).Select(s =>
                    new TestSlotStaffDetailsDto(s.StaffUserId, isArabic ? s.StaffUser!.FullNameAr : s.StaffUser!.FullNameEn,
                        isArabic ? s.Role!.NameAr : s.Role!.NameEn)).ToList(),
                IsCurrentUserRoomHead = currentUserId != Guid.Empty && unitOfWork.Context.Set<TestSlotStaff>()
                    .Any(s => s.TestSlotId == x.Id && s.IsActive && s.RoleId == TestSlotStaffRoleIds.HallSupervisor &&
                              s.StaffUserId == currentUserId),
                HasRevealableAccessCode = x.AccessCodeHash != null,
            }).FirstOrDefaultAsync(ct);
        return slot is null ? Result.Fail<TestSlotDetailsDto>(ErrorsCodes.ItemNotFound) : Result.Ok(slot);
    }
}
