using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Features.Operations.Admin.Roles.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.Admin.Roles.Handlers.Commands;

public sealed class DeleteRoleCommandHandler(RoleManager<ApplicationRole> roleManager)
    : IRequestHandler<DeleteRoleCommand, IResult<Unit>>
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
            ? Result.Ok<Unit>(Unit.Value)
            : RoleClaimSync.FailureFromIdentity(deleteResult);
    }
}
