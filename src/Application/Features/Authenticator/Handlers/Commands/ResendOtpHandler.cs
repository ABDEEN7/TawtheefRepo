using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.HttpClients;
using Tawtheef.Application.Features.Authenticator.Commands;
using Tawtheef.Domain.Configurations.Settings;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

public sealed class ResendOtpHandler(
    ISmsGatewayClient sms,
    IOptions<HodhodSmsSettings> opt,
    TimeProvider time,
    UserManager<User> userManager,
    IUnitOfWork uow)
    : IRequestHandler<ResendOtpCommand, Result>
{
    public async Task<Result> Handle(ResendOtpCommand request, CancellationToken ct)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(u => u.Email == request.Email, ct);
        if (user is null) return Result.Failure(ErrorsCodes.UserNotFound);
        if (string.IsNullOrWhiteSpace(user.PhoneNumber))
            return Result.Failure("USER_PHONE_REQUIRED");

        var now = time.GetUtcNow().DateTime;

        // Cooldown: 60 seconds between resends
        if (user.OtpExpiry is not null && user.OtpExpiry > now && (user.OtpExpiry.Value - now) > TimeSpan.FromMinutes(4))
            return Result.Failure("OTP_RESEND_TOO_FAST");

        // Same throttle as send
        if (user.OtpSends >= 3 && user.OtpExpiry is not null && user.OtpExpiry > now.AddMinutes(-15))
            return Result.Failure("OTP_SEND_LIMIT_REACHED");

        var gen = await sms.GenerateOtpAsync(user.PhoneNumber!, ct);
        if (gen.IsFailure) return gen.ConvertFailure();

        var expires = now.AddMinutes(opt.Value.DefaultOtpMinutes);
        user.SetOtp(gen.Value, expires);

        await uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}
