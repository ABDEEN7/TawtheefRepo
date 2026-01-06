using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Features.Operations.Admin.Offices.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.Admin.Offices.Handlers.Commands;

public sealed class BlockOfficeUserCommandHandler(UserManager<User> userManager)
    : ICommandHandler<BlockOfficeUserCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(BlockOfficeUserCommand request, CancellationToken cancellationToken)
    {
        var officeUser = await userManager.Users.OfType<OfficeUser>()
            .FirstOrDefaultAsync(u => u.Id == request.UserId &&u.OfficeId == request.OfficeId, cancellationToken);

        if (officeUser is null)
            return Result.Fail<Unit>(ErrorsCodes.OfficeUserNotFound);

        officeUser.Block();
        var updateResult = await userManager.UpdateAsync(officeUser);
        if (!updateResult.Succeeded)
            return Result.Fail<Unit>(string.Join(", ", updateResult.Errors.Select(e => e.Description)));

        return Result.Ok(Unit.Value);
    }
}
