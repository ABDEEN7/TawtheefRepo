using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Mappers;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;
using Tawtheef.Application.Features.Authenticator.Handlers.Commands.CallbackHandler;
using Tawtheef.Application.Features.Recruitment.Profile;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Infrastructure.Services.Authorization;

public sealed class ProfileCompletenessService(
    UserManager<User> userManager,
    IUnitOfWork uow,
    IMapper mapper,
    IMediaUrlResolver media
) : IProfileCompletenessService
{
    
    public async Task<ProfileStatusDto> EvaluateAsync(Guid userId, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null) return new ProfileStatusDto();
        
        var profile = await UserProfileLoader.GetFullProfile(uow, userId, ct: ct) ?? new UserProfile {UserId = userId};

        var prefill = await BuildPrefillAsync(user, ct);
        user.Email = (user.Email?.Contains(ConstantQatarPass.PlaceholderEmailDomain) ?? true) ? null : user.Email;
        user.FullNameAr = user.FullNameAr.Contains(ConstantQatarPass.DefaultDisplayName) ? string.Empty : user.FullNameAr;
        user.FullNameEn = user.FullNameEn.Contains(ConstantQatarPass.DefaultDisplayName) ? string.Empty : user.FullNameEn;

        using var scope = new MapContextScope();
        scope.Context.Parameters[ResourceMapper.MediaKey] = media;
        return mapper.Map<ProfileStatusDto>(new ProfileBootstrapSource(profile, user, prefill));
    }

    public async Task<ProfilePrefillDto> BuildPrefillAsync(User user, CancellationToken ct)
    {
        // We use *persisted* claims you already upserted in your handlers
        var claims = await userManager.GetClaimsAsync(user);
        string? C(string type) => claims.FirstOrDefault(c => c.Type == type)?.Value;

        // Google claims to come as "google:xxx", QatarPass as "qatarpass:xxx" (from your code)
        var email      = C("google:email");
        var fullName   = C("google:name");
        var picture    = C("google:picture") ?? user.Avatar;
        var locale     = C("google:locale");
        var qpQid      = C("qatarpass:qid");
        var qpMobile   = C("qatarpass:mobile");
        var qpNat      = C("qatarpass:nationality");


        return new ProfilePrefillDto
        {
            Email        = email,
            EmailVerified = user.EmailConfirmed,
            FullName = fullName,
            Avatar       = picture,
            Locale       = locale,
            Qid          = qpQid,
            Phone    = qpMobile,
            PhoneVerified = user.PhoneNumberConfirmed,
            Nationality  = qpNat,
            Provider     = qpQid is not null ? "qatarpass" : "google"
        };
    }
}
