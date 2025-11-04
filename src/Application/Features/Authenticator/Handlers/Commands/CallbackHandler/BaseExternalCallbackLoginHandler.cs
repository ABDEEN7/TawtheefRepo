using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Authenticator.Handlers.Commands.CallbackHandler;

public abstract class BaseExternalCallbackLoginHandler
{
    protected static async Task<Result<AuthResponse>> IssueTokensAsync<T>(
        T user,
        UserManager<T> userManager,
        ITokenService tokenService,
        IUnitOfWork uow,
        CancellationToken ct) where T : User
    {
        // load UserType nav (like your Google handler)
        user.UserType = await uow.GetEntityRepository<UserType>().DbSet
            .FirstAsync(t => t.Id == user.UserTypeId, ct);

        await tokenService.RevokeAllAsync(user.Id, ct);
        await userManager.UpdateSecurityStampAsync(user);
        var securityStamp = await userManager.GetSecurityStampAsync(user);

        var accessToken =
            tokenService.GenerateAccessToken(user, [new Claim(JwtRegisteredClaimNames.Sid, securityStamp)]);
        var refreshToken = tokenService.GenerateRefreshToken(user.Id, securityStamp);

        return Result.Success(new AuthResponse(
            new UserInfoResponse(user.Id, user.GivenNameEn, user.FamilyNameEn, user.Email!, user.Avatar),
            new TokenResponse(accessToken.Token, accessToken.Expires, refreshToken.Token, refreshToken.Expires)
        ));
    }
}
