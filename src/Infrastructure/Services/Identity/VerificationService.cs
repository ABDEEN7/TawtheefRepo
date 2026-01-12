using Microsoft.Extensions.Caching.Memory;
using Tawtheef.Application.Common.Interfaces.Services.Security;

namespace Tawtheef.Infrastructure.Services.Identity;

public class VerificationService(
    IMemoryCache cache,
    TimeProvider time)
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
}
