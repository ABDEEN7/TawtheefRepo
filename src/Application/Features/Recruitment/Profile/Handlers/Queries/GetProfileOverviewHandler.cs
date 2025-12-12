using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Features.Operations.Employee.ProfileApprovals.DTOs;
using Tawtheef.Application.Features.Recruitment.Profile.DTOs;
using Tawtheef.Application.Features.Recruitment.Profile.Queries;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Queries;

public class GetProfileOverviewHandler(
    IUnitOfWork uow,
    IMapper mapper,
    IMediaUrlResolver media) : IRequestHandler<GetProfileOverviewQuery, Result<ProfileOverviewDto>>
{
    public async Task<Result<ProfileOverviewDto>> Handle(GetProfileOverviewQuery request, CancellationToken ct)
    {
        var profileRepo = uow.GetEntityRepository<UserProfile>();

        var profile = await profileRepo.DbSet
            .Include(p => p.User)
            .Include(p => p.SponsorProfile!.SponsorCard)
            .Include(p => p.SponsorProfile!.SponsorType)
            .Include(p => p.Office)
            .Include(p => p.BirthdayCertificate)
            .Include(p => p.MarriageCertificate)
            .Include(p => p.ResidenceAddress)
            .Include(p => p.CandidateType)
            .Include(p => p.TargetEntity)
            .Include(p => p.Nationality)
            .Include(p => p.Gender)
            .Include(p => p.Religion)
            .Include(p => p.MaritalStatus)
            .Include(p => p.Qualifications)!.ThenInclude(q => q.Major)
            .Include(p => p.Qualifications)!.ThenInclude(q => q.University)
            .Include(p => p.Qualifications)!.ThenInclude(q => q.Certificate)
            .Include(p => p.Experiences)!.ThenInclude(e => e.Certificate)
            .Include(p => p.TrainingCourses)!.ThenInclude(t => t.Certificate)
            .Include(p => p.Achievements)!.ThenInclude(a => a.AchievementType)
            .Include(p => p.Achievements)!.ThenInclude(a => a.Attachment)
            .Include(p => p.Skills)!.ThenInclude(s => s.Skill)
            .Include(p => p.Languages)!.ThenInclude(l => l.Language!)
            .Include(p => p.AdditionalAttachments)!.ThenInclude(a => a.Attachment)
            .FirstOrDefaultAsync(p => p.UserId == request.UserId, ct);

        if (profile is null)
            return Result.Fail<ProfileOverviewDto>(ErrorsCodes.UserProfileNotFound);

        var reviewRepo = uow.GetEntityRepository<ReviewItem>();
        var reviewItems = await reviewRepo.DbSet
            .Where(r => r.UserProfileId == profile.Id
                        && r.Status != ReviewStatus.Approved)
            .Include(r => r.ProfileChange)
            .OrderByDescending(r => r.Version)
            .ToListAsync(ct);

        var latestItems = reviewItems
            .GroupBy(r => new { r.TargetType, r.Section, r.FieldPath, r.EntityName, r.EntityId, r.ResourceId, r.ProfileChangeId })
            .Select(g => g.First())
            .ToList();

        var resourceIds = latestItems
            .Where(r => r.ResourceId.HasValue)
            .Select(r => r.ResourceId!.Value)
            .Distinct()
            .ToList();

        var resourceRepo = uow.GetEntityRepository<Resource>();
        var resources = await resourceRepo.DbSet
            .Where(r => resourceIds.Contains(r.Id))
            .ToDictionaryAsync(r => r.Id, ct);

        var groupedSections = latestItems
            .GroupBy(item => item.Section)
            .Select(group =>
            {
                var items = group
                    .Select(item =>
                    {
                        var dto = mapper.Map<ProfileApprovalItemDto>(item);

                        if (item.ResourceId.HasValue && resources.TryGetValue(item.ResourceId.Value, out var resource))
                        {
                            dto.ResourceUrl = media.ResolveAbsolute(resource.Url);
                        }

                        return dto;
                    })
                    .ToList();

                return new ProfileOverviewSectionDto
                {
                    Section = group.Key,
                    PendingItems = items
                };
            })
            .Where(section => section.PendingItems.Any())
            .OrderBy(s => (int)s.Section)
            .ToList();

        var approvedSnapshot = mapper.Map<ProfileApprovalDataDto>(profile);

        return Result.Ok(new ProfileOverviewDto
        {
            UserProfileId = profile.Id,
            Status = profile.Status,
            ApprovedProfile = approvedSnapshot,
            Sections = groupedSections
        });
    }
}
