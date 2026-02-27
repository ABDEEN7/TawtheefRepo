namespace Application.Recruitment.Features.Profile.Policies;

public static class VerifiedIdentityProviders
{
    public static bool IsLockedProvider(string? provider)
    {
        if (string.IsNullOrWhiteSpace(provider))
            return false;

        return provider.Equals("qatarpass", StringComparison.OrdinalIgnoreCase) ||
               provider.Equals("qatarresidentotp", StringComparison.OrdinalIgnoreCase);
    }
}
