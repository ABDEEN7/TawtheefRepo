using System.Globalization;
using System.Security.Claims;
using Application.Recruitment.Common.Interfaces.Services;
using Application.Recruitment.Features.Authenticator.Commands.QatarLogin;
using Application.Recruitment.Features.Authenticator.Handlers.Utils;
using MediatR;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Logging;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Application.Common.Utils;
using Tawtheef.Application.Features.Authenticator.DTOs;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.Users;

namespace Application.Recruitment.Features.Authenticator.Handlers.Commands.QatarResidentOtp;

public sealed class VerifyQatarResidentOtpCommandHandler(
    IUnitOfWork uow, IMoiService moiService, UserManager<User> userManager,
    ITokenService tokenService, TimeProvider timeProvider, IAppLogger logger,
    IIdentityFieldProtectionContext identityFieldProtectionContext
) : IRequestHandler<VerifyQatarResidentOtpCommand, IResult<AuthResponse>>
{

    private readonly IAppLogger _log = logger.ForContext(typeof(VerifyQatarResidentOtpCommandHandler));

    public async Task<IResult<AuthResponse>> Handle(
        VerifyQatarResidentOtpCommand request,
        CancellationToken cancellationToken)
    {
        // IMPORTANT: do not log OTP / full QID / full phone.
        var normalizedQid = QidUtilities.Normalize(request.Qid);
        var qidMasked = MoiUtils.MaskQid(normalizedQid);

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

        var personalInfoResult = await moiService.GetMoiPersonalInfoWithKawaderCheckAsync(
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
        using var trustedIdentityWriteScope = identityFieldProtectionContext.BeginTrustedIdentityWriteScope();

        var nameUpdate = await UpdateUserAsync(user, personalInfo);
        if (nameUpdate.IsFailed)
        {
            _log.Error(
                "Failed to update user name after MOI lookup. UserId={UserId} Qid={QidMasked} Errors={Errors}",
                user.Id,
                qidMasked,
                string.Join(" | ", nameUpdate.Errors.Select(e => e.Message)));

            return Result.Fail<AuthResponse>(nameUpdate.Errors);
        }

        await UpdateUserProfileAsync(user.Id, normalizedQid, request.QidExpiry, personalInfo, cancellationToken);

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

        return tokens;
    }

    private async Task UpdateUserProfileAsync(
        Guid userId,
        string qidNumber,
        DateOnly expiryDate,
        MOEPersonalInfo personalInfo,
        CancellationToken cancellationToken)
    {
        var qidMasked = MoiUtils.MaskQid(qidNumber);

        var repo = uow.GetEntityRepository<UserProfile>();
        var userProfile = await repo.DbSet.FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);

        if (userProfile is null)
        {
            // create new profile and fill data
            userProfile = new UserProfile()
            {
                UserId = userId,
                Provider = QatarResidentOtpConstants.Provider,
            };
            await repo.DbSet.AddAsync(userProfile, cancellationToken);
        }

        userProfile.NationalNumber = qidNumber;
        userProfile.QIDExpiry = expiryDate;
        userProfile.BirthDate = personalInfo.DateOfBirth;
        userProfile.GenderId = ResolveGenderId(personalInfo.Gender);

        var nationalityId = await ResolveNationalityIdAsync(personalInfo.NationalityCode, cancellationToken);
        userProfile.NationalityId = nationalityId ?? userProfile.NationalityId;

        if (personalInfo.NationalityCode == MoiUtils.QatarNationalityCode)
            userProfile.CandidateTypeId = CandidateTypeIds.Qatari;

        await uow.SaveChangesAsync(cancellationToken);

        _log.Information("UserProfile updated. UserId={UserId} Qid={QidMasked} Expiry={Expiry}", userId, qidMasked, expiryDate);
    }

    private async Task<IResult<Unit>> UpdateUserAsync(User user, MOEPersonalInfo personalInfo)
    {
        var englishName = personalInfo.EnglishFullName.Trim();
        var arabicName = personalInfo.ArabicFullName.Trim();
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

        if (user is ApplicantUser applicantUser)
        {
            var newIsKawader = personalInfo.NationalityCode == MoiUtils.QatarNationalityCode;
            if (applicantUser.IsUserKawader != newIsKawader)
            {
                applicantUser.IsUserKawader = newIsKawader;
                updated = true;
            }
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

    private static Result<T> FailureFromIdentity<T>(IdentityResult res) =>
        Result.Fail<T>(string.Join(", ", res.Errors.Select(e => e.Description)));

    private Guid ResolveGenderId(string genderCode)
    {
        return genderCode == "MALE"? GenderIds.Male : GenderIds.Female;
    }

    public async Task<Guid?> ResolveNationalityIdAsync(int nationalityCode, CancellationToken cancellationToken)
    {
        var nationality = await uow.GetEntityRepository<Country>()
            .DbSet.AsNoTracking().FirstOrDefaultAsync(country => country.Code == nationalityCode, cancellationToken);
        return nationality?.Id;
    }
}

