using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Authenticator.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities;
using Tawtheef.Domain.Entities.Auth;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Authenticator.Handlers.Commands;

public class LogoutHandler(
    IUnitOfWork uow,
    TimeProvider time,
    UserManager<User> userManager,
    SignInManager<User> signInManager) : IRequestHandler<LogoutCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user is null)
            return Result.Failure<Unit>(ErrorsCodes.UserNotFound);


        var activeTokens = await uow.GetEntityRepository<RefreshToken>().DbSet
            .Where(rt => rt.UserId == user.Id).ToListAsync(cancellationToken);
        activeTokens.ForEach(rt=> rt.Revoked(time.GetUtcNow().UtcDateTime, "User logout"));
        
        await userManager.UpdateSecurityStampAsync(user);
        await uow.SaveChangesAsync(cancellationToken);
        
        await signInManager.SignOutAsync();
        return Result.Success(Unit.Value);
    }
}
