using System.Globalization;
using System.Security.Claims;
using Application.Recruitment.Features.Authenticator.Commands.QatarLogin;
using Application.Recruitment.Features.Profile.Queries;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Application.Common.Utils;
using Tawtheef.Application.Features.Authenticator.DTOs;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Kawader;
using Tawtheef.Domain.Entities.Users;
using CheckProfileMOI = Application.Recruitment.Features.Authenticator.DTOs.CheckProfileMOI;

namespace Application.Recruitment.Features.Authenticator.Handlers.Commands.QatarResidentOtp;

public sealed class VerifyQatarResidentOtpCommandHandler(
    IUnitOfWork uow,
    IMediator mediator,
    UserManager<User> userManager,
    ITokenService tokenService,
    TimeProvider timeProvider,
    ILogger logger
) : ICommandHandler<VerifyQatarResidentOtpCommand, IResult<AuthResponse>>
{
    private const int QatarNationalityCode = 634;

    private readonly ILogger _log = logger.ForContext<VerifyQatarResidentOtpCommandHandler>();

    public async Task<IResult<AuthResponse>> Handle(
        VerifyQatarResidentOtpCommand request,
        CancellationToken cancellationToken)
    {
        // IMPORTANT: do not log OTP / full QID / full phone.
        var normalizedQid = QidUtilities.Normalize(request.Qid);
        var qidMasked = MaskQid(normalizedQid);

        _log.Information(
            "Verify Qatar resident OTP started. Qid={QidMasked} Expiry={Expiry}",
            qidMasked,
            request.QidExpiry);

        if (!QidUtilities.IsValid(normalizedQid))
        {
            _log.Warning("Invalid QID format. Qid={QidMasked}", qidMasked);
            return Result.Fail<AuthResponse>(ErrorsCodes.QatarResidentInvalidQid);
        }

        var normalizedPhone = NormalizePhone(request.PhoneNumber);
        if (string.IsNullOrWhiteSpace(normalizedPhone))
        {
            _log.Warning("Phone number missing/invalid. Qid={QidMasked}", qidMasked);
            return Result.Fail<AuthResponse>(ErrorsCodes.UserPhoneRequired);
        }

        var user = await userManager.FindByLoginAsync(QatarResidentOtpConstants.Provider, normalizedQid);
        if (user is null)
        {
            _log.Warning("User not found by provider login. Provider={Provider} Qid={QidMasked}",
                QatarResidentOtpConstants.Provider, qidMasked);

            return Result.Fail<AuthResponse>(ErrorsCodes.UserNotFound);
        }

        var storedPhone = NormalizePhone(user.PhoneNumber ?? string.Empty);
        if (!string.Equals(storedPhone, normalizedPhone, StringComparison.Ordinal))
        {
            _log.Warning("Phone mismatch for QID. UserId={UserId} Qid={QidMasked}", user.Id, qidMasked);
            return Result.Fail<AuthResponse>(ErrorsCodes.QatarResidentPhoneMismatch);
        }

        var nowUtc = timeProvider.GetUtcNow().UtcDateTime;

        // DO NOT log OTP or attempt counter values from user object if it can be abused.
        var otpCheck = user.ValidateOtp(
            request.Otp,
            nowUtc,
            QatarResidentOtpConstants.MaxOtpAttempts,
            QatarResidentOtpConstants.OtpLockDuration);

        var persistOtp = await userManager.UpdateAsync(user);
        if (!persistOtp.Succeeded)
        {
            _log.Error(
                "Failed to persist OTP state after validation. UserId={UserId} Errors={Errors}",
                user.Id,
                string.Join(", ", persistOtp.Errors.Select(e => e.Description)));

            return FailureFromIdentity<AuthResponse>(persistOtp);
        }

        if (otpCheck.IsFailed)
        {
            _log.Warning(
                "OTP validation failed. UserId={UserId} Qid={QidMasked} Errors={Errors}",
                user.Id,
                qidMasked,
                string.Join(" | ", otpCheck.Errors.Select(e => e.Message)));

            return Result.Fail<AuthResponse>(otpCheck.Errors);
        }

        user.PhoneNumberConfirmed = true;
        var update = await userManager.UpdateAsync(user);
        if (!update.Succeeded)
        {
            _log.Error(
                "Failed to confirm phone after OTP success. UserId={UserId} Errors={Errors}",
                user.Id,
                string.Join(", ", update.Errors.Select(e => e.Description)));

            return FailureFromIdentity<AuthResponse>(update);
        }

        var personalInfoResult = await GetMoiPersonalInfoAsync(
            user.Id,
            normalizedQid,
            request.QidExpiry,
            cancellationToken);

        if (personalInfoResult.IsFailed)
        {
            _log.Warning(
                "Login blocked by Kawader check. UserId={UserId} Qid={QidMasked} Errors={Errors}",
                user.Id,
                qidMasked,
                string.Join(" | ", personalInfoResult.Errors.Select(e => e.Message)));

            return Result.Fail<AuthResponse>(personalInfoResult.Errors);
        }

        var personalInfo = personalInfoResult.Value;
        var nameUpdate = await UpdateUserFullNameAsync(user, personalInfo);
        if (nameUpdate.IsFailed)
        {
            _log.Error(
                "Failed to update user name after MOI lookup. UserId={UserId} Qid={QidMasked} Errors={Errors}",
                user.Id,
                qidMasked,
                string.Join(" | ", nameUpdate.Errors.Select(e => e.Message)));

            return Result.Fail<AuthResponse>(nameUpdate.Errors);
        }

        await UpdateUserProfileAsync(user.Id, normalizedQid, request.QidExpiry, cancellationToken);

        var upsert = await UpsertQatarPassClaimsAsync(user, request, normalizedPhone);
        if (upsert.IsFailed)
        {
            _log.Error(
                "Upsert qatarresidentotp claims failed. UserId={UserId} Qid={QidMasked} Errors={Errors}",
                user.Id,
                qidMasked,
                string.Join(" | ", upsert.Errors.Select(e => e.Message)));

            return Result.Fail<AuthResponse>(upsert.Errors);
        }

        var tokens = await tokenService.IssueTokensAsync(user, QatarResidentOtpConstants.Provider, cancellationToken);

        if (tokens.IsFailed)
        {
            _log.Warning(
                "Token issuance failed. UserId={UserId} Provider={Provider} Errors={Errors}",
                user.Id,
                QatarResidentOtpConstants.Provider,
                string.Join(" | ", tokens.Errors.Select(e => e.Message)));
        }
        else
        {
            _log.Information(
                "Verify Qatar resident OTP succeeded. UserId={UserId} Provider={Provider}",
                user.Id,
                QatarResidentOtpConstants.Provider);
        }

        return tokens;
    }

    private async Task<IResult<MOEPersonalInfo>> GetMoiPersonalInfoAsync(
        Guid userId,
        string qid,
        DateOnly expiryDate,
        CancellationToken cancellationToken)
    {
        var qidMasked = MaskQid(qid);

        var request = await mediator.SendQueryAsync<GetPersonalInformationByQidQuery, IResult<MOEPersonalInfo>>(
            new GetPersonalInformationByQidQuery(userId, new CheckProfileMOI(qid, expiryDate)),
            cancellationToken);

        if (request.IsFailed)
        {
            _log.Warning(
                "MOI personal info query failed. UserId={UserId} Qid={QidMasked} Errors={Errors}",
                userId,
                qidMasked,
                string.Join(" | ", request.Errors.Select(e => e.Message)));

            return Result.Fail<MOEPersonalInfo>(request.Errors);
        }

        if (request.Value.NationalityCode != QatarNationalityCode)
            return Result.Ok(request.Value);

        var allowLogin = await CheckIfAllowLoginAsync();
        return allowLogin
            ? Result.Ok(request.Value)
            : Result.Fail<MOEPersonalInfo>(ErrorsCodes.QatariPeopleNotAllowedLoginBeforeRegisterOnKawader);

        async Task<bool> CheckIfAllowLoginAsync()
        {
            var isKawaderUser = await uow.GetEntityRepository<KawaderQid>()
                .DbSet.AsNoTracking()
                .AnyAsync(x => x.Qid == qid, cancellationToken);

            _log.Information(
                "Kawader allow-login check. UserId={UserId} Qid={QidMasked} IsKawaderUser={IsKawaderUser}",
                userId,
                qidMasked,
                isKawaderUser);

            return isKawaderUser;
        }
    }

    private async Task UpdateUserProfileAsync(
        Guid userId,
        string qidNumber,
        DateOnly expiryDate,
        CancellationToken cancellationToken)
    {
        var qidMasked = MaskQid(qidNumber);

        var userProfile = await uow.GetEntityRepository<UserProfile>()
            .DbSet.FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);

        if (userProfile is null)
        {
            // keeping your current behavior (no create). Just log.
            _log.Information("UserProfile not found; skipping update. UserId={UserId} Qid={QidMasked}", userId, qidMasked);
            return;
        }

        userProfile.NationalNumber = qidNumber;
        userProfile.QIDExpiry = expiryDate;
        await uow.SaveChangesAsync(cancellationToken);

        _log.Information("UserProfile updated. UserId={UserId} Qid={QidMasked} Expiry={Expiry}", userId, qidMasked, expiryDate);
    }

    private async Task<IResult<Unit>> UpdateUserFullNameAsync(User user, MOEPersonalInfo personalInfo)
    {
        var englishName = personalInfo.EnglishFullName?.Trim();
        var arabicName = personalInfo.ArabicFullName?.Trim();
        var updated = false;

        if (!string.IsNullOrWhiteSpace(englishName) &&
            !string.Equals(user.FullNameEn, englishName, StringComparison.Ordinal))
        {
            user.FullNameEn = englishName;
            updated = true;
        }

        if (!string.IsNullOrWhiteSpace(arabicName) &&
            !string.Equals(user.FullNameAr, arabicName, StringComparison.Ordinal))
        {
            user.FullNameAr = arabicName;
            updated = true;
        }

        if (!updated)
            return Result.Ok(Unit.Value);

        var update = await userManager.UpdateAsync(user);
        return update.Succeeded ? Result.Ok(Unit.Value) : FailureFromIdentity<Unit>(update);
    }

    private async Task<IResult<Unit>> UpsertQatarPassClaimsAsync(
        User user,
        VerifyQatarResidentOtpCommand data,
        string? normalizedPhone)
    {
        var existing = await userManager.GetClaimsAsync(user);

        var claims = new (string Key, string? Value)[]
        {
            ("qid", data.Qid),
            ("qidExpiry", data.QidExpiry.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
            ("mobile", normalizedPhone)
        };

        foreach (var (key, value) in claims)
        {
            var type = $"qatarresidentotp:{key}";
            var current = existing.FirstOrDefault(c => c.Type == type);

            if (string.IsNullOrWhiteSpace(value))
            {
                if (current is not null)
                    await userManager.RemoveClaimAsync(user, current);
                continue;
            }

            var next = new Claim(type, value);

            if (current is null)
                await userManager.AddClaimAsync(user, next);
            else if (current.Value != value)
                await userManager.ReplaceClaimAsync(user, current, next);
        }

        user.PhoneNumberConfirmed = true;
        var update = await userManager.UpdateAsync(user);
        return update.Succeeded ? Result.Ok() : FailureFromIdentity<Unit>(update);
    }

    private static string NormalizePhone(string phone)
    {
        var digits = new string(phone.Where(char.IsDigit).ToArray());
        if (string.IsNullOrWhiteSpace(digits)) return string.Empty;

        if (digits.StartsWith("974") && digits.Length == 11) return "+" + digits;
        if (digits.Length == 8) return "+974" + digits;

        return "+" + digits;
    }

    private static string MaskQid(string? qid)
    {
        if (string.IsNullOrWhiteSpace(qid)) return "—";
        // keep last 3 digits only
        var digits = new string(qid.Where(char.IsDigit).ToArray());
        if (digits.Length <= 3) return "***";
        return new string('*', digits.Length - 3) + digits[^3..];
    }

    private static Result<T> FailureFromIdentity<T>(IdentityResult res) =>
        Result.Fail<T>(string.Join(", ", res.Errors.Select(e => e.Description)));
}
