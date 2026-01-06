using Cortex.Mediator.Queries;
using FluentResults;
using Mapster;
using MapsterMapper;

using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Mappers;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;
using Tawtheef.Application.Features.Recruitment.Profile.Queries;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Queries;

public sealed class GetMyProfileStatusHandler(IUnitOfWork uow, IMapper mapper, IMediaUrlResolver media)
    : IQueryHandler<GetMyProfileStatusQuery, Result<ProfileStatusDto>>
{
    public async Task<Result<ProfileStatusDto>> Handle(GetMyProfileStatusQuery request, CancellationToken ct)
    {
        var query = BuildSectionQuery(request.Section);

        var profile = await query.FirstOrDefaultAsync(p => p.UserId == request.UserId, ct);
        if (profile is null)
        {
            var user = await uow.GetEntityRepository<User>().DbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == request.UserId, ct);

            return Result.Ok(new ProfileStatusDto
            {
                AgreedToTerms = user?.AgreedToTerms ?? false
            });
        }

        using var scope = new MapContextScope();
        scope.Context.Parameters[ResourceMapper.MediaKey] = media;

        var source = new ProfileBootstrapSource(profile, profile.User!, new ProfilePrefillDto());
        var dto = mapper.Map<ProfileStatusDto>(source);
        return Result.Ok(dto);
    }

    private IQueryable<UserProfile> BuildSectionQuery(ProfileSection? section)
    {
        var repo = uow.GetEntityRepository<UserProfile>();

        var query = repo.DbSet
            .AsSplitQuery()
            .Include(p => p.User)
            .Include(p => p.CandidateType)
            .Include(p => p.TargetEntity)
            .Include(p => p.Office)
            .Include(p => p.Nationality)
            .Include(p => p.Gender)
            .Include(p => p.Religion)
            .Include(p => p.MaritalStatus)
            .Include(p => p.ResidenceCountry)
            .Include(p => p.InterviewLocation)
            .Include(p => p.ResidenceAddress).ThenInclude(r => r!.Certificate)
            .Include(p => p.ResumeAttachment)
            .Include(p => p.NationalCard)
            .Include(p => p.BirthdayCertificate)
            .Include(p => p.MarriageCertificate)
            .Include(p => p.SponsorProfile).ThenInclude(s => s!.SponsorCard)
            .Include(p => p.SponsorProfile).ThenInclude(s => s!.SponsorType)
            .AsQueryable();

        return section switch
        {
            ProfileSection.Qualifications => query
                .Include(p => p.Qualifications)!.ThenInclude(q => q.Degree)
                .Include(p => p.Qualifications)!.ThenInclude(q => q.Country)
                .Include(p => p.Qualifications)!.ThenInclude(q => q.Major)
                .Include(p => p.Qualifications)!.ThenInclude(q => q.SubMajor)
                .Include(p => p.Qualifications)!.ThenInclude(q => q.University)
                .Include(p => p.Qualifications)!.ThenInclude(q => q.Rating)
                .Include(p => p.Qualifications)!.ThenInclude(q => q.StudyType)
                .Include(p => p.Qualifications)!.ThenInclude(q => q.Certificate),
            ProfileSection.Experience or ProfileSection.TrainingCourses => query
                .Include(p => p.Experiences)!.ThenInclude(e => e.Country)
                .Include(p => p.Experiences)!.ThenInclude(e => e.Qualification)
                .Include(p => p.Experiences)!.ThenInclude(e => e.Certificate)
                .Include(p => p.TrainingCourses)!.ThenInclude(t => t.Country)
                .Include(p => p.TrainingCourses)!.ThenInclude(t => t.Certificate),
            ProfileSection.CertificatesAndAwards => query
                .Include(p => p.Achievements)!.ThenInclude(a => a.AchievementType)
                .Include(p => p.Achievements)!.ThenInclude(a => a.Country)
                .Include(p => p.Achievements)!.ThenInclude(a => a.Attachment),
            ProfileSection.Skills => query
                .Include(p => p.Skills)!.ThenInclude(s => s.Skill)
                .Include(p => p.Skills)!.ThenInclude(s => s.Level),
            ProfileSection.Languages => query
                .Include(p => p.Languages)!.ThenInclude(l => l.Language)
                .Include(p => p.Languages)!.ThenInclude(l => l.SpeakingLevel)
                .Include(p => p.Languages)!.ThenInclude(l => l.WritingLevel)
                .Include(p => p.Languages)!.ThenInclude(l => l.ReadingLevel),
            ProfileSection.Attachments => query
                .Include(p => p.AdditionalAttachments)!.ThenInclude(a => a.Attachment),
            _ => query
        };
    }
}
