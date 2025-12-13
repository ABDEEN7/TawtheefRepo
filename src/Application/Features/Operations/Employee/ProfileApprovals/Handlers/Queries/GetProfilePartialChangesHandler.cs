using FluentResults;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Mappers;
using Tawtheef.Application.Features.Operations.Employee.ProfileApprovals.DTOs;
using Tawtheef.Application.Features.Operations.Employee.ProfileApprovals.Queries;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileApprovals.Handlers.Queries;

public class GetProfilePartialChangesHandler(
    IUnitOfWork uow,
    IMapper mapper,
    IMediaUrlResolver media)
    : IRequestHandler<GetProfilePartialChangesQuery, Result<ProfileApprovalDetailDto>>
{
    public async Task<Result<ProfileApprovalDetailDto>> Handle(GetProfilePartialChangesQuery request, CancellationToken ct)
    {
        var profileRepo = uow.GetEntityRepository<UserProfile>();

        var profileQuery = profileRepo.DbSet
            .AsNoTracking()
            .AsSplitQuery()
            .Include(p => p.User)
            .Include(p => p.CandidateType)
            .Include(p => p.TargetEntity)
            .Include(p => p.SponsorProfile!.SponsorCard)
            .Include(p => p.SponsorProfile!.SponsorType)
            .Include(p => p.Office)
            .Include(p => p.BirthdayCertificate)
            .Include(p => p.MarriageCertificate)
            .Include(p => p.ResidenceAddress)
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
            .Include(p => p.AdditionalAttachments)!.ThenInclude(a => a.Attachment);

        var profile = await profileQuery.FirstOrDefaultAsync(p => p.Id == request.UserProfileId, ct);

        if (profile is null)
            return Result.Fail<ProfileApprovalDetailDto>(ErrorsCodes.UserProfileNotFound);

        var assignmentRepo = uow.GetEntityRepository<ProfileAssignment>();
        var isAssigned = await assignmentRepo.DbSet
            .AsNoTracking()
            .AnyAsync(a => a.UserProfileId == profile.Id && a.EmployeeId == request.OfficerId && a.IsActive, ct);

        if (!isAssigned)
            return Result.Fail<ProfileApprovalDetailDto>(ErrorsCodes.UnauthorizedAction);

        var reviewRepo = uow.GetEntityRepository<ReviewItem>();
        var reviewItems = await reviewRepo.DbSet
            .AsNoTracking()
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
            .AsNoTracking()
            .Where(r => resourceIds.Contains(r.Id))
            .ToDictionaryAsync(r => r.Id, ct);

        var reviewItemDtos = latestItems
            .Select(item =>
            {
                var dto = mapper.Map<ProfileApprovalItemDto>(item);

                if (item.ResourceId.HasValue && resources.TryGetValue(item.ResourceId.Value, out var resource))
                {
                    dto.ResourceUrl = media.ResolveAbsolute(resource.Url);
                }

                return (Item: item, Dto: dto);
            })
            .ToList();

        var sections = reviewItemDtos
            .GroupBy(x => x.Item.Section)
            .Select(group =>
            {
                var sectionReview = group
                    .Where(x => x.Item.TargetType == ReviewTargetType.Section)
                    .Select(x => x.Dto)
                    .FirstOrDefault();

                var entries = group
                    .Where(x => x.Item.TargetType != ReviewTargetType.Section)
                    .Select(x => x.Dto)
                    .ToList();

                return new ProfileApprovalSectionDto
                {
                    Section = group.Key,
                    SectionReview = sectionReview,
                    Items = entries,
                    HasAttachments = entries.Any(e => e.TargetType == ReviewTargetType.Attachment)
                };
            })
            .Where(section => section.SectionReview is not null || section.Items.Any())
            .OrderBy(s => (int)s.Section)
            .ToList();

        using var scope = new MapContextScope();
        scope.Context.Parameters[ResourceMapper.MediaKey] = media;
        var profileData = mapper.Map<ProfileApprovalDataDto>(profile);

        var dto = new ProfileApprovalDetailDto
        {
            UserProfileId = profile.Id,
            UserId = profile.UserId,
            FullName = profile.User?.FullNameEn ?? profile.User?.FullNameAr ?? string.Empty,
            CandidateType = profile.CandidateType?.NameAr ?? profile.CandidateType?.NameEn,
            TargetEntity = profile.TargetEntity?.NameAr ?? profile.TargetEntity?.NameEn,
            Profile = profileData,
            Sections = sections,
            IsPartialReview = true
        };

        var auditRepo = uow.GetEntityRepository<AuditTrailEntry>();
        await auditRepo.AddAsync(new AuditTrailEntry
        {
            UserProfileId = profile.Id,
            UserId = request.OfficerId,
            ActionType = "OpenProfilePartialReview",
            Notes = "Profile opened for partial change review",
            Section = nameof(ProfileSection.Personal)
        });
        await uow.SaveChangesAsync(ct);

        return Result.Ok(dto);
    }
}
