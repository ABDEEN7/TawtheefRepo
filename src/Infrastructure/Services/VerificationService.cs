using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Tawtheef.Application.Common.Interfaces.NotificationServices;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;
using Tawtheef.Domain.Configurations;
using Tawtheef.Domain.Configurations.Settings;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Infrastructure.Services;

public class VerificationService(
    IMemoryCache cache,
    IOptions<AppConfigSettings> appConfig,
    IEmailService emailService,
    TimeProvider time,
    UserManager<User> userManager)
    : IVerificationService
{
    private static string AttemptsKey(string email) => $"verification_attempts_{email}";
    private static string LastSentKey(string email) => $"last_verification_sent_{email}";

    public async Task LogVerificationAttempt(string email)
    {
        var attempts = await cache.GetOrCreateAsync(AttemptsKey(email), entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24);
            return Task.FromResult(new List<DateTimeOffset>());
        });

        attempts!.Add(time.GetUtcNow());
        // Re-set is optional (list is a reference), but harmless:
        cache.Set(AttemptsKey(email), attempts);
    }

    public bool CanResendEmail(string email)
    {
        var lastSent = cache.Get<DateTimeOffset?>(LastSentKey(email));
        return !lastSent.HasValue || (time.GetUtcNow() - lastSent.Value) > TimeSpan.FromMinutes(1);
    }

    public int GetResendCooldown(string email)
    {
        var lastSent = cache.Get<DateTimeOffset?>(LastSentKey(email));
        if (!lastSent.HasValue) return 0;

        var elapsed = (time.GetUtcNow() - lastSent.Value).TotalSeconds;
        var cooldown = 60 - (int)elapsed;
        return Math.Max(0, cooldown);
    }

    public async Task<Result<UnverifiedEmailData>> SendVerificationEmail(string email, string? recipientName)
    {
        if (!CanResendEmail(email))
            return Result.Failure<UnverifiedEmailData>(ErrorsCodes.ResendCooldownActive);

        var token = await GenerateEmailVerificationToken(email);
        if (token.IsFailure) return Result.Failure<UnverifiedEmailData>(token.Error);
        if (string.IsNullOrWhiteSpace(appConfig.Value.FrontendUrl))
            return Result.Failure<UnverifiedEmailData>(ErrorsCodes.SiteUrlNotConfigured);

        var callbackUrl =
            $"{appConfig.Value.FrontendUrl}/auth/verify-account?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(token.Value)}";

        await emailService.SendConfirmationLinkEmailAsync(email, recipientName, callbackUrl);

        cache.Set(LastSentKey(email), time.GetUtcNow(), TimeSpan.FromMinutes(1));
        return Result.Success(new UnverifiedEmailData(email, CanResendEmail(email), GetResendCooldown(email)));
    }

    public async Task<Result<string>> GenerateEmailVerificationToken(string email)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return Result.Failure<string>(ErrorsCodes.UserNotFound);
        if (user.EmailConfirmed)
            return Result.Failure<string>(ErrorsCodes.EmailAlreadyVerified);

        var raw = await userManager.GenerateEmailConfirmationTokenAsync(user);
        return Result.Success(raw);
    }
}
