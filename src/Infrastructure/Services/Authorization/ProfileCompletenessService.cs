using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Mappers;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;
using Tawtheef.Application.Features.Authenticator.Handlers.Commands.CallbackHandler;
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
        
        var repo = uow.GetEntityRepository<UserProfile>();
        var profile = await repo.DbSet
            .AsNoTracking().AsSplitQuery()
            .Include(p => p.CandidateType)
            .Include(p => p.TargetEntity)
            .Include(p => p.Office)
            .Include(p => p.ResumeAttachment)
            .Include(p => p.NationalCard)
            .Include(p => p.BirthdayCertificate)
            .Include(p => p.MarriageCertificate)

            .Include(p => p.Nationality)
            .Include(p => p.Gender)
            .Include(p => p.Religion)
            .Include(p => p.MaritalStatus)
            .Include(p => p.SponsorProfile).ThenInclude(s => s!.SponsorType)
            .Include(p => p.SponsorProfile).ThenInclude(s => s!.SponsorCard)

            .Include(p => p.ResidenceCountry)
            .Include(p => p.InterviewLocation)
            .Include(p => p.ResidenceAddress).ThenInclude(a => a!.Certificate)

            .Include(p => p.Qualifications)!.ThenInclude(q => q.Degree)
            .Include(p => p.Qualifications)!.ThenInclude(q => q.Country)
            .Include(p => p.Qualifications)!.ThenInclude(q => q.University)
            .Include(p => p.Qualifications)!.ThenInclude(q => q.Major)
            .Include(p => p.Qualifications)!.ThenInclude(q => q.SubMajor)
            .Include(p => p.Qualifications)!.ThenInclude(q => q.Rating)
            .Include(p => p.Qualifications)!.ThenInclude(q => q.StudyType)
            .Include(p => p.Qualifications)!.ThenInclude(q => q.Certificate)

            .Include(p => p.Experiences)!.ThenInclude(e => e.Country)
            .Include(p => p.Experiences)!.ThenInclude(e => e.Qualification)
            .Include(p => p.Experiences)!.ThenInclude(e => e.Certificate)

            .Include(p => p.TrainingCourses)!.ThenInclude(t => t.Country)
            .Include(p => p.TrainingCourses)!.ThenInclude(t => t.Certificate)

            .Include(p => p.Achievements)!.ThenInclude(a => a.AchievementType)
            .Include(p => p.Achievements)!.ThenInclude(a => a.Country)
            .Include(p => p.Achievements)!.ThenInclude(a => a.Attachment)

            .Include(p => p.Skills)!.ThenInclude(s => s.Skill)
            .Include(p => p.Skills)!.ThenInclude(s => s.Level)

            .Include(p => p.Languages)!.ThenInclude(l => l.Language)
            .Include(p => p.Languages)!.ThenInclude(l => l.SpeakingLevel)
            .Include(p => p.Languages)!.ThenInclude(l => l.WritingLevel)
            .Include(p => p.Languages)!.ThenInclude(l => l.ReadingLevel)
            
            .Include(p => p.AdditionalAttachments)!.ThenInclude(a => a.Attachment)
            .FirstOrDefaultAsync(p => p.UserId == userId, ct) ?? new UserProfile();

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
