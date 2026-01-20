using System.Globalization;
using System.Security.Claims;
using Application.Recruitment.Features.Authenticator.Commands.QatarLogin;
using Application.Recruitment.Features.Profile.Queries;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
    TimeProvider timeProvider)
    : ICommandHandler<VerifyQatarResidentOtpCommand, IResult<AuthResponse>>
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
        var nowUtc = timeProvider.GetUtcNow().UtcDateTime;
        var otpCheck = user.ValidateOtp(
            request.Otp,
            nowUtc,
            QatarResidentOtpConstants.MaxOtpAttempts,
            QatarResidentOtpConstants.OtpLockDuration);

        var persistOtp = await userManager.UpdateAsync(user);
        if (!persistOtp.Succeeded) return FailureFromIdentity<AuthResponse>(persistOtp);
        
        if (otpCheck.IsFailed) 
            return Result.Fail<AuthResponse>(otpCheck.Errors);

        user.PhoneNumberConfirmed = true;
        var update = await userManager.UpdateAsync(user);
        if (!update.Succeeded) return FailureFromIdentity<AuthResponse>(update);
        
        var loginAllowed = await CheckIfQatarUsingKawaderAsync(user.Id, normalizedQid, request.QidExpiry, cancellationToken);
        if(loginAllowed.IsFailed) return Result.Fail<AuthResponse>(loginAllowed.Errors);
        await UpdateUserProfileAsync(user.Id, normalizedQid, request.QidExpiry, cancellationToken);
        var upsert = await UpsertQatarPassClaimsAsync(user, request, normalizedPhone);
        if (upsert.IsFailed) return Result.Fail<AuthResponse>(upsert.Errors);
        return await tokenService.IssueTokensAsync(user, QatarResidentOtpConstants.Provider, cancellationToken);
    }

    private async Task<IResult<Unit>> CheckIfQatarUsingKawaderAsync(Guid userId, string qid, DateOnly expiryDate, CancellationToken cancellationToken)
    {
        var request = await mediator.SendQueryAsync
            <GetPersonalInformationByQidQuery,IResult<MOEPersonalInfo>>
            (new GetPersonalInformationByQidQuery(userId, new CheckProfileMOI(qid, expiryDate)), cancellationToken);
        if(request.IsFailed) return Result.Fail<Unit>(request.Errors);
        if(request.Value.NationalityCode != 634) return Result.Ok(Unit.Value);
        var allowLogin = await CheckIfAllowLoginAsync();
        return allowLogin ? Result.Ok(Unit.Value) : Result.Fail<Unit>(ErrorsCodes.QatariPeopleNotAllowedLoginBeforeRegisterOnKawader);
        
        async Task<bool> CheckIfAllowLoginAsync()
        {
            var isKawaderUser = await uow.GetEntityRepository<KawaderQid>()
                .DbSet.AsNoTracking().AnyAsync(x => x.Qid == qid, cancellationToken);
            return isKawaderUser;
        }
    }
    
    private async Task UpdateUserProfileAsync(Guid userId, string qidNumber, DateOnly expiryDate, CancellationToken cancellationToken)
    {
        // create user profile if user not have one and update information QID and ExpireDate
        var userProfile = await uow.GetEntityRepository<UserProfile>().
            DbSet.FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
        if (userProfile is not null)
        {
            userProfile.NationalNumber = qidNumber;
            userProfile.QIDExpiry = expiryDate;
            await uow.SaveChangesAsync(cancellationToken);
        }
    }
    
    private async Task<IResult<Unit>> UpsertQatarPassClaimsAsync(User user, VerifyQatarResidentOtpCommand data, string? normalizedPhone)
    {
        var existing = await userManager.GetClaimsAsync(user);

        // Map of claim suffix -> value
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
}
