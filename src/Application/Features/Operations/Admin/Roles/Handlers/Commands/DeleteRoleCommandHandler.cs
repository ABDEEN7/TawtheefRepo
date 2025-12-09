using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Features.Operations.Admin.Roles.Commands;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Application.Features.Operations.Admin.Roles.Handlers.Commands;

public sealed class DeleteRoleCommandHandler(RoleManager<IdentityRole<Guid>> roleManager)
    : IRequestHandler<DeleteRoleCommand, Result>
{
    public async Task<Result> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(request.Id.ToString());
        if (role is null)
            return Result.Fail(ErrorsCodes.RoleNotFound);

        var deleteResult = await roleManager.DeleteAsync(role);
        return deleteResult.Succeeded
            ? Result.Ok()
            : RoleClaimSync.FailureFromIdentity(deleteResult);
    }
}
