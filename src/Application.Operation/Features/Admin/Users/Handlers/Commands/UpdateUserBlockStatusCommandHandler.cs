using Application.Operation.Features.Admin.Users.Commands;
using MediatR;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Admin.Users.Handlers.Commands;

public sealed class UpdateUserBlockStatusCommandHandler(
    UserManager<User> userManager,
    ITokenService tokenService)
    : IRequestHandler<UpdateUserBlockStatusCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(UpdateUserBlockStatusCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.Users
            .FirstOrDefaultAsync(
                u => u.Id == request.UserId &&
                     !u.IsDeleted,
                cancellationToken);

        if (user is null)
            return Result.Fail<Unit>(ErrorsCodes.UserNotFound);

        var roles = await userManager.GetRolesAsync(user);
        if (roles.Contains(nameof(SystemRoleIds.SystemAdmin), StringComparer.OrdinalIgnoreCase))
            return Result.Fail<Unit>(ErrorsCodes.SystemAdminBlockNotAllowed);

        user.IsBlocked = request.IsBlocked;

        var updateResult = await userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
            return Result.Fail<Unit>(string.Join(", ", updateResult.Errors.Select(e => e.Description)));

        if (user.IsBlocked)
        {
            await tokenService.RevokeAllAsync(user.Id, cancellationToken);
        }
        else
        {
            await tokenService.ClearUserCacheAsync(user.Id, cancellationToken);
        }

        return Result.Ok(Unit.Value);
    }
}

