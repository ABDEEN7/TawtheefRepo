using Application.Operation.Features.Employee.TestSlots.Commands;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Application.Common.Security;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Employee.TestSlots.Handlers.Commands;

public sealed class UpdateTestSlotAssignmentsCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<UpdateTestSlotAssignmentsCommand, IResult<Unit>>
{
    public Task<IResult<Unit>> Handle(UpdateTestSlotAssignmentsCommand request, CancellationToken ct)
        => unitOfWork.ExecuteInTransactionAsync<IResult<Unit>>(async token =>
        {
            if (request.TestSlotId == Guid.Empty || request.Assignments is null)
                return Result.Fail<Unit>(ErrorsCodes.InvalidRequest);

            var testSlot = await unitOfWork.GetEntityRepository<TestSlot>().DbSet
                .FirstOrDefaultAsync(x => x.Id == request.TestSlotId, token);
            if (testSlot is null)
                return Result.Fail<Unit>(ErrorsCodes.ItemNotFound);

            var hasCreatePermission = httpContextAccessor.HttpContext?.User
                .HasClaim(RoleClaimTypes.Permission, PermissionKeys.TestSlots.Create) == true;
            if (!hasCreatePermission)
            {
                if (!Guid.TryParse(currentUser.UserId, out var currentUserId) ||
                    !await unitOfWork.Context.Set<TestSlotStaff>().AnyAsync(staff =>
                        staff.TestSlotId == testSlot.Id && staff.StaffUserId == currentUserId &&
                        staff.IsActive && staff.RoleId == TestSlotStaffRoleIds.HallSupervisor, token))
                    return Result.Fail<Unit>(new Error(ErrorsCodes.UnauthorizedAction)
                        .WithMetadata("StatusCode", StatusCodes.Status403Forbidden));
            }

            var staffResult = await TestSlotAssignmentOperations.ReplaceAsync(
                unitOfWork, testSlot, request.Assignments.Staff ?? [], token);
            if (staffResult.IsFailed)
                return Result.Fail<Unit>(staffResult.Errors);

            await unitOfWork.SaveChangesAsync(token);
            return Result.Ok(Unit.Value);
        }, ct);
}
