using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Authenticator.DTOs;
using Tawtheef.Application.Features.Authenticator.Queries;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Authenticator.Handlers.Queries;

public class GetUserSocialAccountsQueryHandler(IUnitOfWork uow, UserManager<User> userManager)
    : IRequestHandler<GetUserSocialAccountsQuery, Result<UserSocialAccounts>>
{
    public async Task<Result<UserSocialAccounts>> Handle(GetUserSocialAccountsQuery request, CancellationToken cancellationToken)
    {
        var userResult = await uow.GetUserRepository<User>().GetByIdAsync(request.UserId);
        if (userResult.IsFailure)
            return Result.Failure<UserSocialAccounts>(userResult.Error);

        var user = userResult.Value;
        if (user is null)
            return Result.Failure<UserSocialAccounts>(ErrorsCodes.UserNotFound);

        var logins = await userManager.GetLoginsAsync(user);
        var claims = await userManager.GetClaimsAsync(user);

        // Helper to pull a namespaced claim if exists
        string? C(string provider, string type)
            => claims.FirstOrDefault(c => c.Type == $"{provider}:{type}")?.Value;

        // Google projection
        var googleLogin = logins.FirstOrDefault(l => l.LoginProvider == "Google");
        var googleEmail = C("google", "email") ?? user.Email;
        var googleName  = C("google", "name");
        // You may extend your DTO to include picture/profile if you want:
        // var googlePic   = C("google", "picture");

        // Outlook projection
        var outlookLogin = logins.FirstOrDefault(l => l.LoginProvider == "AzureAD");
        var azureADEmail = C("azureAD", "email") ?? user.Email; // often null if not granted
        var azureADName  = C("azureAD", "name");

        // Build your response
        // Assuming SocialAccounts(string? providerLogin, string? emailOrName) as in your DTO;
        // If you want richer data, extend the DTO to include Name/Picture per provider.
        var googleDto   = new SocialAccounts(googleLogin, googleEmail ?? googleName ?? user.Email);
        var azureADDto = new SocialAccounts(outlookLogin, azureADEmail ?? azureADName ?? user.Email);

        return Result.Success(new UserSocialAccounts(googleDto, azureADDto));
    }
}
