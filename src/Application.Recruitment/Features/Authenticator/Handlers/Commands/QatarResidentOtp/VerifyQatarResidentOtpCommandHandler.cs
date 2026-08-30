using System.Globalization;
using System.Security.Claims;
using Application.Recruitment.Features.Authenticator.Commands.QatarLogin;
using MediatR;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Logging;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
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
    IUnitOfWork uow,
    IMoiService moiService,
    UserManager<User> userManager,
    ITokenService tokenService,
    TimeProvider timeProvider,
    IAppLogger logger,
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

        var pendingPhone = await userManager.GetAuthenticationTokenAsync(
            user,
            QatarResidentOtpConstants.Provider,
            QatarResidentOtpConstants.PendingPhoneTokenName);

        if (string.IsNullOrWhiteSpace(pendingPhone))
        {
            _log.Warning("Pending phone not found for QID. UserId={UserId} Qid={QidMasked}", user.Id, qidMasked);
            return Result.Fail<AuthResponse>(ErrorsCodes.InvalidCode);
        }

        var normalizedPendingPhone = NormalizePhone(pendingPhone);
        if (!string.Equals(normalizedPendingPhone, normalizedPhone, StringComparison.Ordinal))
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

        if (otpCheck.IsFailed)
        {
            var persistOtp = await userManager.UpdateAsync(user);
            if (!persistOtp.Succeeded)
            {
                _log.Error(
                    "Failed to persist OTP state after validation. UserId={UserId} Errors={Errors}",
                    user.Id,
                    string.Join(", ", persistOtp.Errors.Select(e => e.Description)));

                return FailureFromIdentity<AuthResponse>(persistOtp);
            }

            _log.Warning(
                "OTP validation failed. UserId={UserId} Qid={QidMasked} Errors={Errors}",
                user.Id,
                qidMasked,
                string.Join(" | ", otpCheck.Errors.Select(e => e.Message)));

            return Result.Fail<AuthResponse>(otpCheck.Errors);
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
        var synchronization = await uow.ExecuteInTransactionAsync<IResult<Unit>>(async ct =>
        {
            using (identityFieldProtectionContext.BeginTrustedIdentityWriteScope())
            {
                var userUpdate = await SynchronizeUserAsync(user, personalInfo, normalizedPhone);
                if (userUpdate.IsFailed)
                    return userUpdate;

                await UpdateUserProfileAsync(user.Id, normalizedQid, request.QidExpiry, personalInfo, ct);
            }

            var claimsUpdate = await UpsertQatarResidentClaimsAsync(user, request, normalizedPhone);
            if (claimsUpdate.IsFailed)
                return claimsUpdate;

            var removePendingPhone = await userManager.RemoveAuthenticationTokenAsync(
                user,
                QatarResidentOtpConstants.Provider,
                QatarResidentOtpConstants.PendingPhoneTokenName);

            return removePendingPhone.Succeeded
                ? Result.Ok(Unit.Value)
                : FailureFromIdentity<Unit>(removePendingPhone);
        }, cancellationToken);

        if (synchronization.IsFailed)
        {
            _log.Error(
                "Qatar resident identity synchronization failed. UserId={UserId} Qid={QidMasked} Errors={Errors}",
                user.Id,
                qidMasked,
                string.Join(" | ", synchronization.Errors.Select(e => e.Message)));

            return Result.Fail<AuthResponse>(synchronization.Errors);
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
            userProfile = new UserProfile() { UserId = userId, Provider = QatarResidentOtpConstants.Provider, };
            await repo.DbSet.AddAsync(userProfile, cancellationToken);
        }

        userProfile.NationalNumber = qidNumber;
        userProfile.QIDExpiry = expiryDate;
        userProfile.BirthDate = personalInfo.DateOfBirth;
        userProfile.GenderId = ResolveGenderId(personalInfo.Gender);

        var nationalityId = await ResolveNationalityIdAsync(personalInfo.NationalityCode, cancellationToken);
        userProfile.NationalityId = nationalityId ?? userProfile.NationalityId;

        userProfile.CandidateTypeId = personalInfo.NationalityCode switch
        {
            MoiUtils.QatarNationalityCode => CandidateTypeIds.Qatari,
            MoiUtils.QidHolderNationalityCode => CandidateTypeIds.QidHolder,
            _ => userProfile.CandidateTypeId
        };

        await uow.SaveChangesAsync(cancellationToken);

        _log.Information("UserProfile updated. UserId={UserId} Qid={QidMasked} Expiry={Expiry}", userId, qidMasked,
            expiryDate);
    }

    private async Task<IResult<Unit>> SynchronizeUserAsync(
        User user,
        MOEPersonalInfo personalInfo,
        string normalizedPhone)
    {
        var englishName = personalInfo.EnglishFullName.Trim();
        var arabicName = personalInfo.ArabicFullName.Trim();

        if (!string.IsNullOrWhiteSpace(englishName) &&
            !string.Equals(user.FullNameEn, englishName, StringComparison.Ordinal))
        {
            user.FullNameEn = englishName;
        }

        if (!string.IsNullOrWhiteSpace(arabicName) &&
            !string.Equals(user.FullNameAr, arabicName, StringComparison.Ordinal))
        {
            user.FullNameAr = arabicName;
        }

        if (user is ApplicantUser applicantUser)
        {
            var newIsKawader = personalInfo.NationalityCode == MoiUtils.QatarNationalityCode;
            if (applicantUser.IsUserKawader != newIsKawader)
            {
                applicantUser.IsUserKawader = newIsKawader;
            }
        }

        user.PhoneNumber = normalizedPhone;
        user.PhoneNumberConfirmed = true;

        var update = await userManager.UpdateAsync(user);
        return update.Succeeded ? Result.Ok(Unit.Value) : FailureFromIdentity<Unit>(update);
    }

    private async Task<IResult<Unit>> UpsertQatarResidentClaimsAsync(
        User user,
        VerifyQatarResidentOtpCommand data,
        string normalizedPhone)
    {
        var existing = await userManager.GetClaimsAsync(user);

        var claims = new (string Key, string? Value)[]
        {
            ("qid", data.Qid), ("qidExpiry", data.QidExpiry.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
            ("mobile", normalizedPhone)
        };

        foreach ((string key, string? value) in claims)
        {
            var type = $"qatarresidentotp:{key}";
            var current = existing.FirstOrDefault(c => c.Type == type);

            if (string.IsNullOrWhiteSpace(value))
            {
                if (current is not null)
                {
                    var remove = await userManager.RemoveClaimAsync(user, current);
                    if (!remove.Succeeded)
                        return FailureFromIdentity<Unit>(remove);
                }

                continue;
            }

            var next = new Claim(type, value);

            if (current is null)
            {
                var add = await userManager.AddClaimAsync(user, next);
                if (!add.Succeeded)
                    return FailureFromIdentity<Unit>(add);
            }
            else if (current.Value != value)
            {
                var replace = await userManager.ReplaceClaimAsync(user, current, next);
                if (!replace.Succeeded)
                    return FailureFromIdentity<Unit>(replace);
            }
        }

        return Result.Ok(Unit.Value);
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
        return genderCode == "MALE" ? GenderIds.Male : GenderIds.Female;
    }

    private async Task<Guid?> ResolveNationalityIdAsync(int nationalityCode, CancellationToken cancellationToken)
    {
        var nationality = await uow.GetEntityRepository<Country>()
            .DbSet.AsNoTracking().FirstOrDefaultAsync(country => country.Code == nationalityCode, cancellationToken);
        return nationality?.Id;
    }
}
