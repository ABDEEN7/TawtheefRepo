using Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.Commands;
using Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.DTOs;
using Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.DTOs.ProfileApproval;
using Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals;
using Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.Handlers.Commands;
using Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.Queries;
using System.Text.Json;
using FluentResults;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Interfaces.Services.Resources;
using Tawtheef.Application.Common.Mappers;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.Handlers.Queries;

public class GetProfileApprovalDetailHandler(IUnitOfWork uow, IMapper mapper, 
    IMediaUrlResolver media, ILocalizationService localization)
    : IRequestHandler<GetProfileApprovalDetailQuery, Result<GetProfileApprovalDetailDto>>
{
    public async Task<Result<GetProfileApprovalDetailDto>> Handle(GetProfileApprovalDetailQuery request,
        CancellationToken ct)
    {
        
        var profile = await UserProfileLoader.GetFullProfileByProfileId(uow, request.UserProfileId, ct: ct);
        if (profile is null)
            return Result.Fail<GetProfileApprovalDetailDto>(ErrorsCodes.UserProfileNotFound);

        if (profile.Status == UserProfileStatus.Submitted)
        {
            var startProfileUnderReview = new StartUserProfileReviewHandler(uow);
            await startProfileUnderReview.Handle(new StartUserProfileReviewCommand(request.OfficerId, profile.Id), ct);
        }
        else if (profile.Status != UserProfileStatus.UnderReview)
            return Result.Fail<GetProfileApprovalDetailDto>(ErrorsCodes.ProfileNotReadyForReview);

        var assignmentRepo = uow.GetEntityRepository<ProfileAssignment>();
        var auditRepo = uow.GetEntityRepository<AuditTrailEntry>();
        var loggerRepo = uow.GetEntityRepository<UserProfileLogger>();

        var isAssigned = await assignmentRepo.DbSet
            .AsNoTracking()
            .AnyAsync(a => a.UserProfileId == profile.Id && a.EmployeeId == request.OfficerId && a.IsActive, ct);

        if (!isAssigned)
            return Result.Fail<GetProfileApprovalDetailDto>(ErrorsCodes.UnauthorizedAction);

        // ===== Reviews =====
        var reviewRepo = uow.GetEntityRepository<ReviewItem>();
        var reviewItems = await reviewRepo.DbSet
            .AsNoTracking()
            .Where(r => r.UserProfileId == profile.Id && r.ProfileChangeId == null)
            .ToListAsync(ct);

        reviewItems = ActiveProfileReviewItems.ForFullReview(profile, reviewItems);
        var resourceRepo = uow.GetEntityRepository<Resource>();
        var resourceIds = reviewItems
            .Where(r => r.ResourceId.HasValue)
            .Select(r => r.ResourceId!.Value)
            .ToHashSet();

        var resources = resourceIds.Count == 0
            ? new Dictionary<Guid, string>()
            : await resourceRepo.DbSet
                .AsNoTracking()
                .Where(r => resourceIds.Contains(r.Id))
                .ToDictionaryAsync(r => r.Id, r => media.ResolveAbsolute(r.Url), ct);

        var itemDtos = reviewItems
            .Select(item =>
            {
                var dto = mapper.Map<ProfileApprovalItemDto>(item);
                if (item.ResourceId.HasValue && resources.TryGetValue(item.ResourceId.Value, out var url))
                {
                    dto.ResourceUrl = url;
                }

                return (Entity: item, Dto: dto);
            })
            .ToList();

        var itemDtoMap = itemDtos.ToDictionary(x => x.Entity.Id, x => x.Dto);
        var sectionItems = reviewItems.Where(x => x.TargetType == ReviewTargetType.Section).ToList();
        var detailItems = reviewItems.Where(x => x.TargetType != ReviewTargetType.Section).ToList();
        var detailBySection = detailItems
            .GroupBy(x => x.Section)
            .ToDictionary(g => g.Key, g => g.ToList());

        var sections = ProfileApprovalFlow.Sections
            .OrderBy(s => (int)s)
            .Select(sec =>
            {
                var items = detailBySection.TryGetValue(sec, out var list) ? list : [];

                var secItem = sectionItems.FirstOrDefault(x => x.Section == sec);
                var secReview = secItem != null && itemDtoMap.TryGetValue(secItem.Id, out var secDto) ? secDto : null;

                var itemDtosBySection = items
                    .Select(i => itemDtoMap.TryGetValue(i.Id, out var dto) ? dto : null)
                    .Where(dto => dto != null)
                    .Cast<ProfileApprovalItemDto>()
                    .OrderBy(i => (int)i.TargetType)
                    .ThenBy(i => i.Title)
                    .ToList();

                var status = ResolveStatus(itemDtosBySection, secReview);
                var reviewedAt = itemDtosBySection
                    .Where(i => i.ReviewedAtUtc.HasValue)
                    .OrderByDescending(i => i.ReviewedAtUtc)
                    .Select(i => i.ReviewedAtUtc)
                    .FirstOrDefault() ?? secReview?.ReviewedAtUtc;

                var note = secReview?.Note ?? itemDtosBySection
                    .Select(i => i.Note)
                    .FirstOrDefault(n => !string.IsNullOrWhiteSpace(n));

                return new ProfileApprovalSectionDto
                {
                    Section = sec,
                    Status = status,
                    Note = note,
                    ReviewedAtUtc = reviewedAt,
                    SectionReview = secReview,
                    Items = itemDtosBySection,
                    HasAttachments = itemDtosBySection.Any(i => i.TargetType == ReviewTargetType.Attachment)
                };
            })
            .ToList();

        // ===== Profile data (snapshot) =====
        var profileData = MapProfile(profile);
        var dto = new GetProfileApprovalDetailDto
        {
            UserProfileId = profile.Id,
            UserId = profile.UserId,
            FullName = localization.GetLocalizedFullName(profile.User),
            CandidateType = localization.GetLocalizedName(profile.CandidateType),
            TargetEntity = localization.GetLocalizedName(profile.TargetEntity),
            Profile = profileData,
            ProfileStatus = (int)profile.Status,
            Sections = sections
        };
        var openProfileNote = JsonSerializer.Serialize(new
        {
            eventType = "OpenProfile",
            message = UserProfileLogConstants.Notes.ProfileOpenedForReview
        });

        await auditRepo.AddAsync(new AuditTrailEntry
        {
            UserProfileId = profile.Id,
            UserId = request.OfficerId,
            ActionType = UserProfileLogConstants.ActionTypes.OpenProfile,
            Notes = openProfileNote,
            Section = nameof(ProfileSection.Personal)
        });
        await loggerRepo.AddAsync(new UserProfileLogger
        {
            UserProfileId = profile.Id,
            PerformedById = request.OfficerId,
            ActionType = UserProfileLogConstants.ActionTypes.OpenProfile,
            Notes = openProfileNote,
            Section = nameof(ProfileSection.Personal)
        });
        await uow.SaveChangesAsync(ct);

        return Result.Ok(dto);

        ReviewStatus ResolveStatus(IReadOnlyList<ProfileApprovalItemDto> items, ProfileApprovalItemDto? sectionReview)
        {
            var source = items.Any() ? items : sectionReview != null ? [sectionReview] : [];

            if (source.Any(i => i.Status == ReviewStatus.NeedsCorrection)) return ReviewStatus.NeedsCorrection;
            if (source.Any(i => i.Status == ReviewStatus.Rejected)) return ReviewStatus.Rejected;
            if (source.Any(i => i.Status is ReviewStatus.Pending or ReviewStatus.NotReviewed or ReviewStatus.Solved)) return ReviewStatus.Pending;

            return source.Count == 0 ? ReviewStatus.Pending : ReviewStatus.Approved;
        }

        // ===== Local helpers using mapper =====
        ProfileApprovalDataDto MapProfile(UserProfile profileEntity)
        {
            using var scope = new MapContextScope();
            scope.Context.Parameters[ResourceMapper.MediaKey] = media;
            var result = mapper.Map<ProfileApprovalDataDto>(profileEntity);
            return result;
        }
    }
}

