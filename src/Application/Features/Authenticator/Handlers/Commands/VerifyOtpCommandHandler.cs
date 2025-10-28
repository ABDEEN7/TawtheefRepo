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
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Authenticator.Handlers.Commands;

 public class VerifyOtpCommandHandler(
     IUnitOfWork uow,TimeProvider time,
     UserManager<User> userManager,
     ITokenService tokenService)
     : IRequestHandler<VerifyOtpCommand, Result<AuthResponse>>
 {
     public async Task<Result<AuthResponse>> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
        {
            var user = await userManager.Users.Include(u=> u.UserType)
                .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken: cancellationToken);
            if (user == null)
                return Result.Failure<AuthResponse>(ErrorsCodes.UserNotFound);

            if (user.OtpExpiry < time.GetLocalNow().DateTime)
                return Result.Failure<AuthResponse>(ErrorsCodes.VerificationCodeExpired);

            if (user.OtpAttempts >= 3)
                return Result.Failure<AuthResponse>(ErrorsCodes.TooManyAttempts);

            if (user.OtpCode != request.Otp)
            {
                user.OtpAttempts++;
                await uow.SaveChangesAsync(cancellationToken);

                return Result.Failure<AuthResponse>(ErrorsCodes.InvalidVerificationCode);
            }

            user.OtpCode = null;
            user.OtpExpiry = null;
            user.OtpAttempts = 0;
            user.EmailConfirmed = true;

            var accessToken = tokenService.GenerateAccessToken(user);
            var refreshToken = tokenService.GenerateRefreshToken(user.Id);
        
            user.UpdateLoginInfo(accessToken.Token, time.GetLocalNow().DateTime);
            user.AddRefreshToken(refreshToken.Token, refreshToken.Expires);
            user.RemoveOldRefreshTokens(5);
            
            await uow.SaveChangesAsync(cancellationToken);

            return Result.Success(new AuthResponse(
                new UserInfoResponse(
                    user.Id,
                    user.GivenNameEn,
                    user.FamilyNameEn,
                    user.Email!,
                    user.Avatar
                ),
                new TokenResponse(accessToken.Token,
                    accessToken.Expires,
                    refreshToken.Token,
                    refreshToken.Expires)
            ));
        }
    }
