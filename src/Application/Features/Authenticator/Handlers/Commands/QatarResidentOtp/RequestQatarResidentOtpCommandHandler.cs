using System.Security.Cryptography;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Interfaces.Services.HttpClients;
using Tawtheef.Application.Common.Utilities;
using Tawtheef.Application.Features.Authenticator.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Authenticator.Handlers.Commands.QatarResidentOtp;

public sealed class RequestQatarResidentOtpCommandHandler(
    IQatarResidentVerificationClient verificationClient,
    UserManager<User> userManager,
    ISmsSender smsSender,
    TimeProvider timeProvider)
    : IRequestHandler<RequestQatarResidentOtpCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(RequestQatarResidentOtpCommand request, CancellationToken cancellationToken)
    {
        var normalizedQid = QidUtilities.Normalize(request.Qid);
        if (!QidUtilities.IsValid(normalizedQid))
            return Result.Fail<Unit>(ErrorsCodes.QatarResidentInvalidQid);

        if(!IsQatarMobileNumber(request.PhoneNumber))
            return Result.Fail<Unit>(ErrorsCodes.QatarResidentPhoneInvalid);
        
        var normalizedPhone = NormalizePhone(request.PhoneNumber);
        if (string.IsNullOrWhiteSpace(normalizedPhone))
            return Result.Fail<Unit>(ErrorsCodes.UserPhoneRequired);

        var verification = await verificationClient.VerifyAsync(normalizedQid, normalizedPhone, cancellationToken);
        if (verification.IsFailed) return Result.Fail<Unit>(verification.Errors);

        if (!string.Equals(QidUtilities.Normalize(normalizedQid), normalizedQid, StringComparison.Ordinal))
            return Result.Fail<Unit>(ErrorsCodes.QatarResidentInvalidQid);

        var user = await userManager.FindByLoginAsync(QatarResidentOtpConstants.Provider, normalizedQid);
        var placeholderEmail = $"qr{normalizedQid}{QatarResidentOtpConstants.PlaceholderEmailDomain}";

        if (user is null)
        {
            user = await userManager.FindByEmailAsync(placeholderEmail);
            if (user is null)
            {
                var newUser = User.Register(placeholderEmail, QatarResidentOtpConstants.DisplayName, UserTypeIds.Applicant);
                if (newUser.IsFailed) return Result.Fail<Unit>(newUser.Errors);

                user = newUser.Value;
                user.PhoneNumber = normalizedPhone;
                user.PhoneNumberConfirmed = false;

                var createRes = await userManager.CreateAsync(user);
                if (!createRes.Succeeded) return FailureFromIdentity<Unit>(createRes);
            }

            var addLogin = await userManager.AddLoginAsync(user,
                new UserLoginInfo(QatarResidentOtpConstants.Provider, normalizedQid, QatarResidentOtpConstants.Provider));

            if (!addLogin.Succeeded) return FailureFromIdentity<Unit>(addLogin);
        }
        else
        {
            user.PhoneNumber = normalizedPhone;
            user.PhoneNumberConfirmed = false;
        }

        if (user.OtpSends >= QatarResidentOtpConstants.MaxOtpSends)
            return Result.Fail<Unit>(ErrorsCodes.SendOtpLimitReached);

        var now = timeProvider.GetUtcNow().UtcDateTime;
        var otp = GenerateCode(QatarResidentOtpConstants.OtpLength);
        user.SetOtp(otp, now.AddMinutes(QatarResidentOtpConstants.OtpExpiryMinutes));

        var update = await userManager.UpdateAsync(user);
        if (!update.Succeeded) return FailureFromIdentity<Unit>(update);

        _ = await smsSender.SendAsync(normalizedPhone,
            $"Your verification code is: {otp}", cancellationToken);

        return Result.Ok(Unit.Value);
    }
    
    public static bool IsQatarMobileNumber(string phone) => phone.StartsWith("+974");

    private static string NormalizePhone(string phone)
    {
        var digits = new string(phone.Where(char.IsDigit).ToArray());
        if (string.IsNullOrWhiteSpace(digits)) return string.Empty;

        if (digits.StartsWith("974") && digits.Length == 11) return "+" + digits;
        if (digits.Length == 8) return "+974" + digits;

        return "+" + digits;
    }

    private static string GenerateCode(int length)
    {
        const string digits = "0123456789";
        var bytes = RandomNumberGenerator.GetBytes(length);
        var chars = new char[length];
        for (int i = 0; i < length; i++)
        {
            chars[i] = digits[bytes[i] % digits.Length];
        }

        return new string(chars);
    }

    private static Result<T> FailureFromIdentity<T>(IdentityResult res) =>
        Result.Fail<T>(string.Join(", ", res.Errors.Select(e => e.Description)));
}
