using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Infrastructure.Services.Authorization;

public sealed class ProfileCompletenessService(
    UserManager<User> userManager,
    IUnitOfWork uow,
    IMapper mapper
) : IProfileCompletenessService
{
    
    public async Task<ProfileStatusDto> EvaluateAsync(Guid userId, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null) return new ProfileStatusDto();
        
        var repo = uow.GetEntityRepository<UserProfile>();
        var profile = await repo.DbSet
            .AsNoTracking()
            // Attachments الأساسية
            .Include(p => p.SponsorProfile).ThenInclude(s => s!.SponsorCard)
            .Include(p => p.ResumeAttachment)
            .Include(p => p.NationalCard)
            .Include(p => p.ResidenceAddress)
            .Include(p => p.ResidenceAddressCertificate)
            .Include(p => p.BirthdayCertificate)
            .Include(p => p.MarriageCertificate)
            // Additional attachments
            .Include(p => p.AdditionalAttachments!)
                .ThenInclude(a => a.Attachment)
            // Collections
            .Include(p => p.Skills)!.ThenInclude(sp=> sp.Skill)
            .Include(p => p.Languages)!.ThenInclude(sp=> sp.Language)
            .Include(p => p.Qualifications)!
                .ThenInclude(a => a.University)
            .Include(p => p.Qualifications)!
                .ThenInclude(a => a.Major)
            .Include(p => p.Qualifications)!
                .ThenInclude(a => a.SubMajor)
            .Include(p => p.Qualifications)!
                .ThenInclude(a => a.Certificate)
            .Include(p => p.Experiences)!.ThenInclude(a => a.Certificate)
            .Include(p => p.TrainingCourses)!.ThenInclude(a => a.Certificate)
            .FirstOrDefaultAsync(p => p.UserId == userId, ct);

        if (profile is null)
        {
            return new ProfileStatusDto
            {
                IsComplete = false,
                IsDraft    = false
            };
        }
        var prefill = await BuildPrefillAsync(user, ct);

        return mapper.Map<ProfileStatusDto>((profile, user, prefill));
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
