using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Features.Operations.Admin.Offices.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.Admin.Offices.Handlers.Commands;

public sealed class UpdateOfficeUserBlockStatusCommandHandler(UserManager<User> userManager)
    : IRequestHandler<UpdateOfficeUserBlockStatusCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(UpdateOfficeUserBlockStatusCommand request, CancellationToken cancellationToken)
    {
        var officeUser = await userManager.Users
            .OfType<OfficeUser>()
            .FirstOrDefaultAsync(
                u => u.Id == request.UserId &&
                     u.OfficeId == request.OfficeId &&
                     !u.IsDeleted,
                cancellationToken);

        if (officeUser is null)
            return Result.Fail<Unit>(ErrorsCodes.OfficeUserNotFound);

        officeUser.IsBlocked = request.IsBlocked;

        var updateResult = await userManager.UpdateAsync(officeUser);
        if (!updateResult.Succeeded)
            return Result.Fail<Unit>(string.Join(", ", updateResult.Errors.Select(e => e.Description)));

        return Result.Ok(Unit.Value);
    }
}
