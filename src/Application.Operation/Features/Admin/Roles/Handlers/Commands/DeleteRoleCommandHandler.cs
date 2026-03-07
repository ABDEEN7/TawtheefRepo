using Application.Operation.Features.Admin.Roles.Commands;
using MediatR;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Admin.Roles.Handlers.Commands;

public sealed class DeleteRoleCommandHandler(
    RoleManager<ApplicationRole> roleManager,
    UserManager<User> userManager,
    ITokenService tokenService)
    : IRequestHandler<DeleteRoleCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(request.Id.ToString());
        if (role is null)
            return Result.Fail<Unit>(ErrorsCodes.RoleNotFound);

        if (role.IsSystemRole)
            return Result.Fail<Unit>(ErrorsCodes.SystemRoleModificationNotAllowed);

        var users = await userManager.GetUsersInRoleAsync(role.Name!);
        var deleteResult = await roleManager.DeleteAsync(role);

        if (deleteResult.Succeeded)
        {
            foreach (var user in users)
            {
                await tokenService.ClearUserCacheAsync(user.Id, cancellationToken);
            }
            return Result.Ok(Unit.Value);
        }

        return RoleClaimSync.FailureFromIdentity(deleteResult);
    }
}

