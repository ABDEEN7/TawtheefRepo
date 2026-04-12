using Application.Operation.Features.Admin.Offices.Commands;
using MediatR;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Admin.Offices.Handlers.Commands;

public sealed class BlockOfficeUserCommandHandler(
    UserManager<User> userManager,
    ITokenService tokenService)
    : IRequestHandler<BlockOfficeUserCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(BlockOfficeUserCommand request, CancellationToken cancellationToken)
    {
        var officeUser = await userManager.Users.OfType<OfficeUser>()
            .FirstOrDefaultAsync(u => u.Id == request.UserId &&u.OfficeId == request.OfficeId, cancellationToken);

        if (officeUser is null)
            return Result.Fail<Unit>(ErrorsCodes.OfficeUserNotFound);

        officeUser.IsBlocked = request.IsBlocked;
        var updateResult = await userManager.UpdateAsync(officeUser);
        if (!updateResult.Succeeded)
            return Result.Fail<Unit>(string.Join(", ", updateResult.Errors.Select(e => e.Description)));

        if (officeUser.IsBlocked)
        {
            await tokenService.RevokeAllAsync(officeUser.Id, cancellationToken);
        }
        else
        {
            await tokenService.ClearUserCacheAsync(officeUser.Id, cancellationToken);
        }

        return Result.Ok(Unit.Value);
    }
}

