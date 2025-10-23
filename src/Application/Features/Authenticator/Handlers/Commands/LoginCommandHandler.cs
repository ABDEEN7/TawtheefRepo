using System;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Features.Authenticator.Commands;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Authenticator.Handlers.Commands;

public class LoginCommandHandler(
    UserManager<User> userManager,
    ITokenService tokenService,
    IUnitOfWork uow,TimeProvider time,
    IVerificationService verificationService)
    : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.Users.Include(u => u.UserType)
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
        if (user is null)
            return Result.Failure<AuthResponse>(ErrorsCodes.InvalidCredentials);
        
        if (!await userManager.CheckPasswordAsync(user, request.Password))
            return Result.Failure<AuthResponse>(ErrorsCodes.InvalidCredentials);

        if (!user.EmailConfirmed)
        {
            await verificationService.LogVerificationAttempt(user.Email!);

            return Result.Success(new AuthResponse
            {
                RequiresEmailVerification = true,
                UnverifiedEmail = new UnverifiedEmailData(
                    user.Email!,
                    verificationService.CanResendEmail(user.Email!),
                    verificationService.GetResendCooldown(user.Email!))
            });
        }

        if (string.IsNullOrWhiteSpace(request.DeviceId))
            return Result.Failure<AuthResponse>(ErrorsCodes.InvalidDeviceId);
        

        var accessToken = tokenService.GenerateAccessToken(user);

        var refreshToken = tokenService.GenerateRefreshToken(user.Id, request.IpAddress);

        user.UpdateLoginInfo(accessToken.Token, time.GetLocalNow().DateTime);
        user.AddRefreshToken(refreshToken.Token, refreshToken.Expires, request.IpAddress);
        user.RemoveOldRefreshTokens(5);
        
        await userManager.UpdateAsync(user);
        await uow.SaveChangesAsync(cancellationToken);

        return Result.Success(new AuthResponse(
            new UserInfoResponse(
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email!,
                user.Avatar
            ),
            new TokenResponse(
                accessToken.Token,
                accessToken.Expires,
                refreshToken.Token,
                refreshToken.Expires
            )
        ));
    }
}
