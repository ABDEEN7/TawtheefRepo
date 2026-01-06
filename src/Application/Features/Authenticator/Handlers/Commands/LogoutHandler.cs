using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Features.Authenticator.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Authenticator.Handlers.Commands;

public class LogoutHandler(
    IUnitOfWork uow,
    ITokenService tokenService,
    UserManager<User> userManager,
    SignInManager<User> signInManager) : ICommandHandler<LogoutCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user is null)
            return Result.Fail<Unit>(ErrorsCodes.UserNotFound);
        
        await tokenService.RevokeAllAsync(user.Id, cancellationToken);
        await userManager.UpdateSecurityStampAsync(user);
        await uow.SaveChangesAsync(cancellationToken);
        
        await signInManager.SignOutAsync();
        return Result.Ok(Unit.Value);
    }
}
