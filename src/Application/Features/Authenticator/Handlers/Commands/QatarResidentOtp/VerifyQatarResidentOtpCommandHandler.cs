using System.Linq;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Utilities;
using Tawtheef.Application.Features.Authenticator.Commands;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Authenticator.Handlers.Commands.QatarResidentOtp;

public sealed class VerifyQatarResidentOtpCommandHandler(
    UserManager<User> userManager,
    ITokenService tokenService,
    TimeProvider timeProvider)
    : IRequestHandler<VerifyQatarResidentOtpCommand, IResult<AuthResponse>>
{
    public async Task<IResult<AuthResponse>> Handle(VerifyQatarResidentOtpCommand request, CancellationToken cancellationToken)
    {
        var normalizedQid = QidUtilities.Normalize(request.Qid);
        if (!QidUtilities.IsValid(normalizedQid))
            return Result.Fail<AuthResponse>(ErrorsCodes.QatarResidentInvalidQid);

        var normalizedPhone = NormalizePhone(request.PhoneNumber);
        if (string.IsNullOrWhiteSpace(normalizedPhone))
            return Result.Fail<AuthResponse>(ErrorsCodes.UserPhoneRequired);

        var user = await userManager.FindByLoginAsync(QatarResidentOtpConstants.Provider, normalizedQid);
        if (user is null)
            return Result.Fail<AuthResponse>(ErrorsCodes.UserNotFound);

        var storedPhone = NormalizePhone(user.PhoneNumber ?? string.Empty);
        if (!string.Equals(storedPhone, normalizedPhone, StringComparison.Ordinal))
            return Result.Fail<AuthResponse>(ErrorsCodes.QatarResidentPhoneMismatch);

        var otpCheck = user.ValidateOtp(request.Otp, timeProvider.GetUtcNow().UtcDateTime, QatarResidentOtpConstants.MaxOtpAttempts);

        var persistOtp = await userManager.UpdateAsync(user);
        if (!persistOtp.Succeeded) return FailureFromIdentity<AuthResponse>(persistOtp);
        if (otpCheck.IsFailed) return Result.Fail<AuthResponse>(otpCheck.Errors);

        user.PhoneNumberConfirmed = true;
        var update = await userManager.UpdateAsync(user);
        if (!update.Succeeded) return FailureFromIdentity<AuthResponse>(update);

        return await tokenService.IssueTokensAsync(user, QatarResidentOtpConstants.Provider, cancellationToken);
    }

    private static string NormalizePhone(string phone)
    {
        var digits = new string(phone.Where(char.IsDigit).ToArray());
        if (string.IsNullOrWhiteSpace(digits)) return string.Empty;

        if (digits.StartsWith("974") && digits.Length == 11) return "+" + digits;
        if (digits.Length == 8) return "+974" + digits;

        return "+" + digits;
    }

    private static Result<T> FailureFromIdentity<T>(IdentityResult res) =>
        Result.Fail<T>(string.Join(", ", res.Errors.Select(e => e.Description)));
}
