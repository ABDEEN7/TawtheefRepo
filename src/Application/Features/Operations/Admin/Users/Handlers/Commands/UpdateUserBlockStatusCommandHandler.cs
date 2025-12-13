using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Features.Operations.Admin.Users.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.Admin.Users.Handlers.Commands;

public sealed class UpdateUserBlockStatusCommandHandler(UserManager<User> userManager)
    : IRequestHandler<UpdateUserBlockStatusCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(UpdateUserBlockStatusCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.Users
            .FirstOrDefaultAsync(
                u => u.Id == request.UserId &&
                     u.UserTypeId == UserTypeIds.Employee &&
                     !u.IsDeleted,
                cancellationToken);

        if (user is null)
            return Result.Fail<Unit>(ErrorsCodes.UserNotFound);

        user.IsBlocked = request.IsBlocked;

        var updateResult = await userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
            return Result.Fail<Unit>(string.Join(", ", updateResult.Errors.Select(e => e.Description)));

        return Result.Ok(Unit.Value);
    }
}
