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

namespace Application.Operation.Features.Employee.TestSlots.Handlers.Queries;

public sealed class GetTestSlotConfigurationQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    : IRequestHandler<GetTestSlotConfigurationQuery, IResult<TestSlotConfigurationDto>>
{
    public async Task<IResult<TestSlotConfigurationDto>> Handle(GetTestSlotConfigurationQuery request,
        CancellationToken cancellationToken)
    {
        var scope = await TestSlotReadScope.ResolveAsync(
            unitOfWork,
            currentUser,
            cancellationToken);

        if (!await TestSlotReadScope.CanAccessAsync(
                unitOfWork,
                scope,
                request.TestSlotId,
                cancellationToken))
        {
            return Result.Fail<TestSlotConfigurationDto>(
                new Error(ErrorsCodes.UnauthorizedAction)
                    .WithMetadata(
                        "StatusCode",
                        StatusCodes.Status403Forbidden));
        }

        var isArabic = string.Equals(request.Language, "ar", StringComparison.OrdinalIgnoreCase);
        var slot = await unitOfWork.Context.Set<TestSlot>().AsNoTracking()
            .Where(x => x.Id == request.TestSlotId)
            .Select(x => new TestSlotConfigurationDto
            {
                Id = x.Id,
                TitleAr = x.TitleAr,
                TitleEn = x.TitleEn,
                RoomId = x.RoomId,
                SlotDate = x.SlotDate,
                StartTime = x.StartTime,
                EndTime = x.EndTime,
                Staff = unitOfWork.Context.Set<TestSlotStaff>().AsNoTracking()
                    .Where(staff => staff.TestSlotId == x.Id)
                    .OrderBy(staff => staff.Id)
                    .Select(staff => new TestSlotConfigurationStaffDto
                    {
                        StaffUserId = staff.StaffUserId,
                        Name = isArabic ? staff.StaffUser!.FullNameAr : staff.StaffUser!.FullNameEn,
                        RoleId = staff.RoleId,
                    }).ToList()
            }).FirstOrDefaultAsync(cancellationToken);
        return slot == null
            ? Result.Fail<TestSlotConfigurationDto>(ErrorsCodes.InvalidRequest)
            : Result.Ok(slot);
    }
}
