using Application.Recruitment.Features.Profile.DTOs;
using Application.Recruitment.Features.Profile.Queries;
using MediatR;
using FluentResults;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Resources;
using Tawtheef.Application.Common.Mappers;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

namespace Application.Recruitment.Features.Profile.Handlers.Queries;

public sealed class GetMyProfileDetailHandler(
    IUnitOfWork uow,
    IMapper mapper,
    IMediaUrlResolver media)
    : IRequestHandler<GetMyProfileDetailQuery, Result<MyProfileDetailDto>>
{
    public async Task<Result<MyProfileDetailDto>> Handle(GetMyProfileDetailQuery request, CancellationToken ct)
    {
        if (request.UserId == Guid.Empty)
            return Result.Fail<MyProfileDetailDto>(ErrorsCodes.InvalidUserIdentifier);

        var profileRepo = uow.GetEntityRepository<UserProfile>();

        var profile = await profileRepo.DbSet
            .AsNoTracking()
            .AsSplitQuery()
            .Include(p => p.User)
            .Include(p => p.CandidateType)
            .Include(p => p.TargetEntity)
            .Include(p => p.Office)
            .Include(p => p.BirthdayCertificate)
            .Include(p => p.MarriageCertificate)
            .Include(p => p.NationalCard)
            .Include(p => p.ResumeAttachment)
            .Include(p => p.Nationality)
            .Include(p => p.Gender)
            .Include(p => p.Religion)
            .Include(p => p.MaritalStatus)
            .Include(p => p.SponsorProfile!.SponsorCard)
            .Include(p => p.SponsorProfile!.SponsorType)
            .Include(p => p.ResidenceCountry)
            .Include(p => p.InterviewLocation)
            .Include(p => p.ResidenceAddress)

            .Include(p => p.Qualifications)!.ThenInclude(q => q.Major)
            .Include(p => p.Qualifications)!.ThenInclude(q => q.SubMajor)
            .Include(p => p.Qualifications)!.ThenInclude(q => q.University)
            .Include(p => p.Qualifications)!.ThenInclude(q => q.Certificate)
            .Include(p => p.Qualifications)!.ThenInclude(q => q.StudyType)
            .Include(p => p.Qualifications)!.ThenInclude(q => q.Degree)
            .Include(p => p.Qualifications)!.ThenInclude(q => q.Rating)
            .Include(p => p.Qualifications)!.ThenInclude(q => q.Country)

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

            .Include(p => p.Languages)!.ThenInclude(l => l.Language!)
            .Include(p => p.Languages)!.ThenInclude(l => l.SpeakingLevel!)
            .Include(p => p.Languages)!.ThenInclude(l => l.ReadingLevel!)
            .Include(p => p.Languages)!.ThenInclude(l => l.WritingLevel!)

            .Include(p => p.AdditionalAttachments)!.ThenInclude(a => a.Attachment)

            .FirstOrDefaultAsync(p => p.UserId == request.UserId, ct);

        if (profile is null)
            return Result.Fail<MyProfileDetailDto>(ErrorsCodes.UserProfileNotFound);

        // Map with media resolver (same approach you used)
        using var scope = new MapContextScope();
        scope.Context.Parameters[ResourceMapper.MediaKey] = media;

        var draft = mapper.Map<ProfileApprovalDataDto>(profile);

        // ApprovedProfile: if you donâ€™t have snapshots/pointers yet, keep it simple for now:
        // - if profile is Approved => ApprovedProfile = same as draft (until you implement snapshots)
        // - else null
        ProfileApprovalDataDto? approved = profile.Status == UserProfileStatus.Approved ? draft : null;

        var perms = ResolvePermissions(profile.Status);

        return Result.Ok(new MyProfileDetailDto
        {
            UserId = request.UserId,
            UserProfileId = profile.Id,
            ProfileStatus = profile.Status,
            DraftProfile = draft,
            ApprovedProfile = approved,
            Permissions = perms
        });
    }

    private static ProfileEditPermissionsDto ResolvePermissions(UserProfileStatus status)
    {
        // Your rule: allow change (Prereq/Personal/Contact), and allow add-only elsewhere.
        // Locking is optional; adapt to your workflow.
        var locked = status is UserProfileStatus.UnderReview;

        return new ProfileEditPermissionsDto
        {
            IsLockedBecauseUnderReview = locked,

            CanEditPrerequisites = !locked,
            CanEditPersonal = !locked,
            CanEditContact = !locked,

            CanAddQualifications = !locked,
            CanAddExperiences = !locked,
            CanAddTrainingCourses = !locked,
            CanAddCertificatesAndAwards = !locked,

            CanEditSkills = !locked,
            CanEditLanguages = !locked,
            CanAddAttachments = !locked,

            CanSubmit = !locked
        };
    }
}

