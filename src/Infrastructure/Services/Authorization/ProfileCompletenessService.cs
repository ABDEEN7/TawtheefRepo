using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Infrastructure.Services.Authorization;

public sealed class ProfileCompletenessService(
    UserManager<User> userManager,
    IUnitOfWork uow
) : IProfileCompletenessService
{
    public async Task<(bool isComplete, string[] missing)> EvaluateAsync(Guid userId, CancellationToken ct)
    {
        // Example rule set — adapt to your UserProfile schema
        var profile = await uow.GetEntityRepository<UserProfile>().DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == userId, ct);

        var missing = new List<string>();

        if (profile is null) 
        {
            missing.AddRange(new[]
            {
                "candidateTypeId","targetEntityId","nationalityId","maritalStatusId",
                "birthDate","residenceCountryId","address","passportNo"
            });
            return (false, missing.ToArray());
        }

        // Minimal sample checks — expand as needed
        if (profile.CandidateTypeId == Guid.Empty) missing.Add("candidateTypeId");
        if (profile.TargetEntityId  == Guid.Empty) missing.Add("targetEntityId");
        if (profile.NationalityId   == Guid.Empty) missing.Add("nationalityId");
        if (profile.MaritalStatusId == Guid.Empty) missing.Add("maritalStatusId");
        if (profile.BirthDate == default)          missing.Add("birthDate");
        if (profile.ResidenceCountryId == Guid.Empty) missing.Add("residenceCountryId");

        var isComplete = missing.Count == 0;
        return (isComplete, missing.ToArray());
    }

    public async Task<ProfilePrefillDto?> BuildPrefillAsync(User user, CancellationToken ct)
    {
        // We use *persisted* claims you already upserted in your handlers
        var claims = await userManager.GetClaimsAsync(user);
        string? C(string type) => claims.FirstOrDefault(c => c.Type == type)?.Value;

        // Google claims come as "google:xxx", QatarPass as "qatarpass:xxx" (from your code)
        var email      = C("google:email") ?? user.Email;
        var fullName   = C("google:name");
        var picture    = C("google:picture") ?? user.Avatar;
        var locale     = C("google:locale");
        var qpQid      = C("qatarpass:qid");
        var qpMobile   = C("qatarpass:mobile");
        var qpNat      = C("qatarpass:nationality");


        return new ProfilePrefillDto
        {
            Email        = email,
            FullName = fullName,
            Avatar       = picture,
            Locale       = locale,
            Qid          = qpQid,
            Phone    = qpMobile,
            Nationality  = qpNat,
            Provider     = qpQid is not null ? "qatarpass" : "google"
        };
    }
}
