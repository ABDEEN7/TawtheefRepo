using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Extensions;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;
using Tawtheef.Domain.Entities;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Infrastructure.Services.Authorization;

public sealed class ProfileCompletenessService(
    UserManager<User> userManager,
    IUnitOfWork uow
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
            .Include(p => p.Skills)
            .Include(p => p.Languages)
            .Include(p => p.Qualifications)!.ThenInclude(a => a.Certificate)
            .Include(p => p.Experiences)!.ThenInclude(a => a.Certificate)
            .Include(p => p.TrainingCourses)!.ThenInclude(a => a.Certificate)
            .FirstOrDefaultAsync(p => p.UserId == userId, ct);
        
        var missing = new List<string>();

        // ===== 1) لا يوجد UserProfile إطلاقاً =====
        if (profile is null)
        {
            missing.AddRange(new[]
            {
                "candidateTypeId", "targetEntityId", "nationalityId", "maritalStatusId",
                "birthDate", "residenceCountryId", "address", "nationalNumber"
            });

            return new ProfileStatusDto
            {
                IsComplete = false,
                IsDraft    = false,
                Missing    = missing.ToArray()
            };
        }

        // ===== 2) Profile موجود لكن Draft =====
        if (profile.IsDraft)
        {
            missing.Add("draft");
        }

        // ===== 3) Checks للحقول المطلوبة (مثال، عدّل حسب البزنس) =====
        if (profile.CandidateTypeId == Guid.Empty)            missing.Add("candidateTypeId");
        if (profile.TargetEntityId == Guid.Empty)             missing.Add("targetEntityId");
        if (profile.NationalityId is null
            || profile.NationalityId == Guid.Empty)           missing.Add("nationalityId");
        if (profile.MaritalStatusId is null
            || profile.MaritalStatusId == Guid.Empty)         missing.Add("maritalStatusId");
        if (profile.BirthDate is null)                        missing.Add("birthDate");
        if (profile.ResidenceCountryId is null
            || profile.ResidenceCountryId == Guid.Empty)      missing.Add("residenceCountryId");
        if (string.IsNullOrWhiteSpace(profile.Address))       missing.Add("address");
        if (string.IsNullOrWhiteSpace(profile.NationalNumber)) missing.Add("nationalNumber");

        bool isComplete = missing.Count == 0;
        // Additional attachments
        var additional = profile.AdditionalAttachments?
            .Where(a => a.Attachment != null)
            .Select(a => new AdditionalAttachmentDto
            {
                Id    = a.Id,
                Title = a.FileName,
                File  = ToFileRefNonNull(a.Attachment!)
            })
            .ToList();

        // Qualifications
        var qualifications = profile.Qualifications?
            .Select(q => new QualificationDto
            {
                Id             = q.Id,
                DegreeId       = q.LevelId,
                DegreeName     = q.Level?.GetLocalizedName("ar"),
                Major          = q.Major?.GetLocalizedName("ar"),
                UniversityName = q.University?.GetLocalizedName("ar"),
                GraduationYear = q.GraduationYear,
                Attachment     = ToFileRef(q.Certificate)
            })
            .ToList();

        // Experiences
        var experiences = profile.Experiences?
            .Select(e => new ExperienceDto
            {
                Id           = e.Id,
                EmployerName = e.Organization,
                JobTitle     = e.Position,
                StartDate    = e.StartDate,
                EndDate      = e.EndDate,
                IsCurrent    = e.EndDate is null,
                Attachment   = ToFileRef(e.Certificate)
            })
            .ToList();

        // Training Courses
        var trainings = profile.TrainingCourses?
            .Select(t => new TrainingCourseDto
            {
                Id        = t.Id,
                Provider  = t.Organization,
                Title     = t.Position,
                StartDate = t.StartDate,
                EndDate   = t.EndDate,
                Attachment = ToFileRef(t.Certificate)
            })
            .ToList();

        // Skills
        var skills = profile.Skills?
            .Select(s => new SkillDto
            {
                Id       = s.Id,
                SkillId  = s.SkillId
            })
            .ToList();

        // Languages
        var languages = profile.Languages?
            .Select(l => new LanguageDto
            {
                Id         = l.Id,
                LanguageId = l.LanguageId,
                LevelId    = l.LevelId
            })
            .ToList();

        var prefill = await BuildPrefillAsync(user, ct);

        // ===== Build final DTO =====
        return new ProfileStatusDto
        {
            IsComplete = isComplete,
            IsDraft    = profile.IsDraft,
            Missing    = missing.ToArray(),

            Avatar = user.Avatar ?? prefill.Avatar,
            FullNameAr = string.IsNullOrWhiteSpace(user.FullNameAr) ? prefill.FullName : user.FullNameAr,
            FullNameEn = string.IsNullOrWhiteSpace(user.FullNameEn) ? prefill.FullName : user.FullNameEn,
            Email = user.Email ?? prefill.Email,
            EmailVerified = user.EmailConfirmed,
            Phone = user.PhoneNumber ?? prefill.Phone,
            PhoneVerified = user.PhoneNumberConfirmed,
            
            CandidateTypeId              = profile.CandidateTypeId,
            TargetEntityId               = profile.TargetEntityId,

            NationalNumber               = profile.NationalNumber ?? prefill.Qid,
            BirthDate                    = profile.BirthDate,
            NationalityId                = profile.NationalityId,
            GenderId                     = profile.GenderId,
            ReligionId                   = profile.ReligionId,
            MaritalStatusId              = profile.MaritalStatusId,

            ChildrenCount                = profile.ChildrenCount,

            ResidenceCountryId           = profile.ResidenceCountryId,
            InterviewLocationId          = profile.InterviewLocationId,

            Address                      = profile.Address,
            naBuilding = profile.ResidenceAddress?.BuildingNo,
            naStreet = profile.ResidenceAddress?.StreetNo,
            naZone = profile.ResidenceAddress?.ZoneNo,
            naUnit = profile.ResidenceAddress?.UnitNo,

            HasDisability                = profile.HasDisability,
            DisabilityDetails            = profile.DisabilityDetails,
            
            SponsorEmployerName = profile.SponsorProfile?.SponsorName,
            SponsorEmployerNumber = profile.SponsorProfile?.SponsorNumber,
            SponsorTypeId = profile.SponsorProfile?.SponsorTypeId,
            SponsorCard = ToFileRef(profile.SponsorProfile?.SponsorCard),

            ResumeAttachment             = ToFileRef(profile.ResumeAttachment),
            NationalCard                 = ToFileRef(profile.NationalCard),
            ResidenceAddressCertificate  = ToFileRef(profile.ResidenceAddressCertificate),
            BirthdayCertificate          = ToFileRef(profile.BirthdayCertificate),
            MarriageCertificate          = ToFileRef(profile.MarriageCertificate),
            AdditionalAttachments        = additional,

            Qualifications               = qualifications,
            Experiences                  = experiences,
            TrainingCourses              = trainings,
            Skills                       = skills,
            Languages                    = languages
        };


        // ===== Helpers =====
        static FileRefDto? ToFileRef(Resource? r)
            => r is null ? null : new FileRefDto
            {
                ResourceId = r.Id,
                FileName   = r.Name
            };

        static FileRefDto ToFileRefNonNull(Resource r)
            => new()
            {
                ResourceId = r.Id,
                FileName   = r.Name
            };
    }

    public async Task<ProfilePrefillDto> BuildPrefillAsync(User user, CancellationToken ct)
    {
        // We use *persisted* claims you already upserted in your handlers
        var claims = await userManager.GetClaimsAsync(user);
        string? C(string type) => claims.FirstOrDefault(c => c.Type == type)?.Value;

        // Google claims come as "google:xxx", QatarPass as "qatarpass:xxx" (from your code)
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
