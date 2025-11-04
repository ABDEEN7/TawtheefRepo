using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Interfaces.Services.HttpClients;
using Tawtheef.Application.Features.Authenticator.Commands;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

public class VerifyOtpHandler(
    IUnitOfWork uow,
    TimeProvider time,
    UserManager<User> userManager,
    ITokenService tokenService,
    ISmsGatewayClient sms)  // <— add gateway
    : IRequestHandler<VerifyOtpCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.Users
            .Include(u => u.UserType)
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user is null) return Result.Failure<AuthResponse>(ErrorsCodes.UserNotFound);

        var now = time.GetUtcNow().DateTime;

        if (user.OtpAttempts >= 3)
            return Result.Failure<AuthResponse>(ErrorsCodes.TooManyAttempts);

        if (user.OtpExpiry is null || user.OtpExpiry < now || string.IsNullOrWhiteSpace(user.OtpReference))
            return Result.Failure<AuthResponse>(ErrorsCodes.VerificationCodeExpired);

        var validate = await sms.ValidateOtpAsync(request.Otp, user.OtpReference!, cancellationToken);
        if (validate.IsFailure)
        {
            user.OtpAttempts++;
            await uow.SaveChangesAsync(cancellationToken);
            return Result.Failure<AuthResponse>(ErrorsCodes.InvalidVerificationCode);
        }

        // Success → clear OTP and sign-in
        user.ClearOtp();
        user.EmailConfirmed = true;

        await userManager.UpdateSecurityStampAsync(user);
        var securityStamp = await userManager.GetSecurityStampAsync(user);

        var accessToken = tokenService.GenerateAccessToken(
            user, [new Claim(JwtRegisteredClaimNames.Sid, securityStamp)]
        );
        var refreshToken = tokenService.GenerateRefreshToken(user.Id, securityStamp);

        user.UpdateLoginInfo(accessToken.Token, now);
        user.AddRefreshToken(refreshToken.Token, refreshToken.Expires, securityStamp);
        user.RemoveOldRefreshTokens(5);

        await uow.SaveChangesAsync(cancellationToken);

        return Result.Success(new AuthResponse(
            new UserInfoResponse(user.Id, user.GivenNameEn, user.FamilyNameEn, user.Email!, user.Avatar),
            new TokenResponse(accessToken.Token, accessToken.Expires, refreshToken.Token, refreshToken.Expires)
        ));
    }
}
