using Application.Operation.Features.Employee.TestSlots.DTOs;
using Application.Operation.Features.Employee.TestSlots.Queries;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Employee.TestSlots.Handlers.Queries;

public sealed class GetTestSlotAccessCodeQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser,
    IAccessCodeProtector accessCodeProtector) : IRequestHandler<GetTestSlotAccessCodeQuery, IResult<TestSlotAccessCodeDto>>
{
    public async Task<IResult<TestSlotAccessCodeDto>> Handle(GetTestSlotAccessCodeQuery request, CancellationToken ct)
    {
        if (!Guid.TryParse(currentUser.UserId, out var userId))
            return Forbidden();
        var slot = await unitOfWork.Context.Set<TestSlot>().AsNoTracking().Where(x => x.Id == request.TestSlotId)
            .Select(x => new { x.AccessCodeHash, IsRoomHead = unitOfWork.Context.Set<TestSlotStaff>().Any(s =>
                s.TestSlotId == x.Id && s.IsActive && s.StaffUserId == userId &&
                s.RoleId == TestSlotStaffRoleIds.HallSupervisor) }).FirstOrDefaultAsync(ct);
        if (slot is null) return Result.Fail<TestSlotAccessCodeDto>(ErrorsCodes.ItemNotFound);
        if (!slot.IsRoomHead) return Forbidden();
        if (string.IsNullOrWhiteSpace(slot.AccessCodeHash))
            return Result.Fail<TestSlotAccessCodeDto>("TEST_SLOT_ACCESS_CODE_NOT_AVAILABLE");
        try { return Result.Ok(new TestSlotAccessCodeDto(accessCodeProtector.Unprotect(slot.AccessCodeHash))); }
        catch { return Result.Fail<TestSlotAccessCodeDto>("TEST_SLOT_ACCESS_CODE_NOT_AVAILABLE"); }
    }

    private static IResult<TestSlotAccessCodeDto> Forbidden() => Result.Fail<TestSlotAccessCodeDto>(
        new FluentResults.Error(ErrorsCodes.UnauthorizedAction).WithMetadata("StatusCode", StatusCodes.Status403Forbidden));
}
