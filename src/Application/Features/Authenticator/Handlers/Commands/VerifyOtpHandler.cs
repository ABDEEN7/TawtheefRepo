using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Features.Authenticator.Commands;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Authenticator.Handlers.Commands
{
    public class VerifyOtpHandler(
        IUnitOfWork uow,
        TimeProvider time,
        UserManager<User> userManager,
        ITokenService tokenService)
        : IRequestHandler<VerifyOtpCommand, Result<AuthResponse>>
    {
        public async Task<Result<AuthResponse>> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
        {
            // Fetch user (include UserType because you need it for token claims)
            var user = await userManager.Users
                .Include(u => u.UserType)
                .Include(u => u.RefreshTokens) // optional, if you plan to revoke here
                .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

            if (user is null)
                return Result.Failure<AuthResponse>(ErrorsCodes.UserNotFound);

            var now = time.GetLocalNow().DateTime;

            // Basic OTP guards
            if (user.OtpAttempts >= 3)
                return Result.Failure<AuthResponse>(ErrorsCodes.TooManyAttempts);

            if (user.OtpExpiry is null || user.OtpExpiry < now)
                return Result.Failure<AuthResponse>(ErrorsCodes.VerificationCodeExpired);

            // Compare code
            if (!string.Equals(user.OtpCode, request.Otp, StringComparison.Ordinal))
            {
                user.OtpAttempts++;
                await uow.SaveChangesAsync(cancellationToken);
                return Result.Failure<AuthResponse>(ErrorsCodes.InvalidVerificationCode);
            }

            // Success: clear OTP info + confirm email
            user.OtpCode = null;
            user.OtpExpiry = null;
            user.OtpAttempts = 0;
            user.EmailConfirmed = true;

            // ---- SINGLE SESSION ENFORCEMENT ----
            // Rotate the security stamp to invalidate any prior sessions
            await userManager.UpdateSecurityStampAsync(user);
            var securityStamp = await userManager.GetSecurityStampAsync(user);

            // Issue tokens tagged with the current security stamp (sid)
            var accessToken = tokenService.GenerateAccessToken(
                user,
                [new Claim(JwtRegisteredClaimNames.Sid, securityStamp)]
            );

            var refreshToken = tokenService.GenerateRefreshToken(user.Id, securityStamp);

            // Persist login + refresh
            user.UpdateLoginInfo(accessToken.Token, now);
            user.AddRefreshToken(refreshToken.Token, refreshToken.Expires, securityStamp);
            user.RemoveOldRefreshTokens(5); // keep last N if you want an audit tail

            await uow.SaveChangesAsync(cancellationToken);

            return Result.Success(new AuthResponse(
                new UserInfoResponse(user.Id,user.GivenNameEn,user.FamilyNameEn, user.Email!,user.Avatar),
                new TokenResponse(accessToken.Token,accessToken.Expires,refreshToken.Token,refreshToken.Expires)
            ));
        }
    }
}
