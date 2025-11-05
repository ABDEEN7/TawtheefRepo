using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.HttpClients;
using Tawtheef.Application.Features.Authenticator.Commands;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

public class VerifyOtpHandler(
    IUnitOfWork uow,
    TimeProvider time,
    UserManager<User> userManager,
    ISmsGatewayClient sms)
    : IRequestHandler<VerifyOtpCommand, Result<VerifyOtpResponse>>
{
    public async Task<Result<VerifyOtpResponse>> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.Users
            .Include(u => u.UserType)
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user is null) return Result.Failure<VerifyOtpResponse>(ErrorsCodes.UserNotFound);

        var now = time.GetUtcNow().DateTime;

        if (user.OtpAttempts >= 3)
            return Result.Failure<VerifyOtpResponse>(ErrorsCodes.TooManyAttempts);

        if (user.OtpExpiry is null || user.OtpExpiry < now || string.IsNullOrWhiteSpace(user.OtpReference))
            return Result.Failure<VerifyOtpResponse>(ErrorsCodes.VerificationCodeExpired);

        var validate = await sms.ValidateOtpAsync(request.Otp, user.OtpReference!, cancellationToken);
        if (validate.IsFailure)
        {
            user.OtpAttempts++;
            await uow.SaveChangesAsync(cancellationToken);
            return Result.Failure<VerifyOtpResponse>(ErrorsCodes.InvalidVerificationCode);
        }
        return Result.Success(new VerifyOtpResponse());
    }
}
