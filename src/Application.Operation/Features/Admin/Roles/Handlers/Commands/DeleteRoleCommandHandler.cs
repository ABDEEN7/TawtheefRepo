using Application.Operation.Features.Admin.Roles.Commands;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Admin.Roles.Handlers.Commands;

public sealed class DeleteRoleCommandHandler(RoleManager<ApplicationRole> roleManager)
    : ICommandHandler<DeleteRoleCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(request.Id.ToString());
        if (role is null)
            return Result.Fail<Unit>(ErrorsCodes.RoleNotFound);

        if (role.IsSystemRole)
            return Result.Fail<Unit>(ErrorsCodes.SystemRoleModificationNotAllowed);

        var deleteResult = await roleManager.DeleteAsync(role);
        return deleteResult.Succeeded
            ? Result.Ok(Unit.Value)
            : RoleClaimSync.FailureFromIdentity(deleteResult);
    }
}
