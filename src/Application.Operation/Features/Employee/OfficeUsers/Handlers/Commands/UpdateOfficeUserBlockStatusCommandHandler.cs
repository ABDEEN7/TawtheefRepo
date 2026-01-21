using Application.Operation.Features.Employee.OfficeUsers.Commands;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.OfficeUsers.Handlers.Commands;

public sealed class UpdateOfficeUserBlockStatusCommandHandler(
    UserManager<User> userManager,
    ICurrentUserService currentUserService)
    : ICommandHandler<UpdateOfficeUserBlockStatusCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(
        UpdateOfficeUserBlockStatusCommand request,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(currentUserService.UserId, out var currentUserId))
            return Result.Fail<Unit>(ErrorsCodes.InvalidUserIdentifier);

        var officeAdmin = await userManager.Users
            .OfType<OfficeUser>()
            .FirstOrDefaultAsync(user => user.Id == currentUserId && !user.IsDeleted, cancellationToken);

        if (officeAdmin?.OfficeId is null)
            return Result.Fail<Unit>(ErrorsCodes.OfficeAdminNotFound);

        var officeUser = await userManager.Users
            .OfType<OfficeUser>()
            .FirstOrDefaultAsync(
                user => user.Id == request.UserId && user.OfficeId == officeAdmin.OfficeId && !user.IsDeleted,
                cancellationToken);

        if (officeUser is null)
            return Result.Fail<Unit>(ErrorsCodes.OfficeUserNotFound);

        officeUser.IsBlocked = request.IsBlocked;

        var updateResult = await userManager.UpdateAsync(officeUser);
        if (!updateResult.Succeeded)
            return Result.Fail<Unit>(ErrorsCodes.OfficeAdminCreationFailed);

        return Result.Ok(Unit.Value);
    }
}
