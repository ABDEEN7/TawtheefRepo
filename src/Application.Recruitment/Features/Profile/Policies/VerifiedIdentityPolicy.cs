using FluentResults;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

namespace Application.Recruitment.Features.Profile.Policies;

public static class VerifiedIdentityPolicy
{
    public static Result EnsureIdentityHydrated(User user, UserProfile profile)
    {
        if (!VerifiedIdentityProviders.IsLockedProvider(profile.Provider))
            return Result.Ok();

        if (string.IsNullOrWhiteSpace(user.FullNameAr) ||
            string.IsNullOrWhiteSpace(user.FullNameEn) ||
            string.IsNullOrWhiteSpace(profile.NationalNumber) ||
            profile.BirthDate is null ||
            profile.NationalityId is null ||
            profile.GenderId is null)
        {
            return Result.Fail(ErrorsCodes.VerifiedIdentityNotReady);
        }

        return Result.Ok();
    }
}
