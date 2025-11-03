using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.HttpClients;
using Tawtheef.Application.Features.Authenticator.Commands;
using Tawtheef.Domain.Configurations;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

public sealed class SendOtpHandler(
    ISmsGatewayClient sms,
    IOptions<HodhodSmsSettings> opt,
    TimeProvider time,
    UserManager<User> userManager,
    IUnitOfWork uow)
    : IRequestHandler<SendOtpCommand, Result>
{
    public async Task<Result> Handle(SendOtpCommand request, CancellationToken ct)
    {
        var user = await userManager.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, ct);

        if (user is null) return Result.Failure(ErrorsCodes.UserNotFound);
        if (string.IsNullOrWhiteSpace(user.PhoneNumber))
            return Result.Failure(ErrorsCodes.UserPhoneRequired);

        // Throttling: max 3 sends per 15 minutes
        if (user.OtpSends >= 3 && user.OtpExpiry is not null && user.OtpExpiry > time.GetUtcNow().AddMinutes(-15).DateTime)
            return Result.Failure(ErrorsCodes.SendOtpLimitReached);

        var gen = await sms.GenerateOtpAsync(user.PhoneNumber!, ct);
        if (gen.IsFailure) return gen.ConvertFailure();

        var expires = time.GetUtcNow().AddMinutes(opt.Value.DefaultOtpMinutes).DateTime;
        user.SetOtp(gen.Value, expires);

        await uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}
