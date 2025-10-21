using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Authenticator.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Authenticator.Handlers.Commands;

public class UnlinkSocialAccountCommandHandler(IUnitOfWork uow,SignInManager<User> signInManager, UserManager<User> userManager) : IRequestHandler<UnlinkSocialAccountCommand, Result>
{
    public async Task<Result> Handle(UnlinkSocialAccountCommand request, CancellationToken cancellationToken)
    {
        var userResult = await uow.GetUserRepository<User>().GetByIdAsync(request.UserId);
        if (userResult.IsFailure)
            return Result.Failure(userResult.Error);
        
        var user = userResult.Value;
        if (user is null)
            return Result.Failure(ErrorsCodes.UserNotFound);

        var logins = await userManager.GetLoginsAsync(user);
        var login = logins.FirstOrDefault(l => string.Equals(l.LoginProvider,request.Provider, StringComparison.InvariantCultureIgnoreCase));

        if (login == null)
            return Result.Failure(ErrorsCodes.SocialAccountNotLinked);

        var result = await userManager.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey);

        if (!result.Succeeded)
            return Result.Failure(ErrorsCodes.UnlinkSocialAccountFailed);

        // Refresh sign-in so the cookie is updated
        await signInManager.RefreshSignInAsync(user);

        return Result.Success();
    }
}
