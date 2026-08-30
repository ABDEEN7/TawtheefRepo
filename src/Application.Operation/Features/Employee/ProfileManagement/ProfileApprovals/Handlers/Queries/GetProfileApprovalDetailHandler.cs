using Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.Commands;
using Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.DTOs;
using Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.DTOs.ProfileApproval;
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
using Tawtheef.Application.Common.Services;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.Handlers.Queries;

public class GetProfileApprovalDetailHandler(
    IUnitOfWork uow,
    IMapper mapper,
    IMediaUrlResolver media,
    ILocalizationService localization)
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
            var startResult = await startProfileUnderReview.Handle(
                new StartUserProfileReviewCommand(request.OfficerId, profile.Id),
                ct);
            if (startResult.IsFailed)
                return Result.Fail<GetProfileApprovalDetailDto>(startResult.Errors);
        }
        else
        {
            if (profile.Status != UserProfileStatus.UnderReview)
                return Result.Fail<GetProfileApprovalDetailDto>(ErrorsCodes.ProfileNotReadyForReview);

            var isAssigned = await uow.GetEntityRepository<ProfileAssignment>().DbSet
                .AsNoTracking()
                .AnyAsync(assignment =>
                        assignment.UserProfileId == profile.Id &&
                        assignment.EmployeeId == request.OfficerId &&
                        assignment.IsActive,
                    ct);

            if (!isAssigned)
                return Result.Fail<GetProfileApprovalDetailDto>(ErrorsCodes.UnauthorizedAction);
        }

        var auditRepo = uow.GetEntityRepository<AuditTrailEntry>();
        var loggerRepo = uow.GetEntityRepository<UserProfileLogger>();

        var previousFinalizePayload = await loggerRepo.DbSet
            .AsNoTracking()
            .Where(log =>
                log.UserProfileId == profile.Id &&
                log.ActionType == UserProfileLogConstants.ActionTypes.ProfileReviewFinalized &&
                log.Notes != null)
            .OrderByDescending(log => log.CreatedDate)
            .Select(log => log.Notes)
            .FirstOrDefaultAsync(ct);

        var internalReviewerNote = ReadInternalReviewerNote(previousFinalizePayload);
        var sectionNotePayloads = await loggerRepo.DbSet
            .AsNoTracking()
            .Where(log =>
                log.UserProfileId == profile.Id &&
                log.ActionType == UserProfileLogConstants.ActionTypes.ProfileSectionInternalNote &&
                log.Section != null &&
                log.Notes != null)
            .OrderByDescending(log => log.CreatedDate)
            .Select(log => new { log.Section, log.Notes })
            .ToListAsync(ct);
        var internalNotesBySection = sectionNotePayloads
            .GroupBy(log => log.Section!, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                group => group.Key,
                group => ReadSectionInternalNote(group.First().Notes),
                StringComparer.OrdinalIgnoreCase);

        await ProfileReviewItemSync.EnsureConditionalAttachmentItemsAsync(uow, profile, ct);
        foreach (var section in ProfileApprovalFlow.Sections)
        {
            await FullReviewSectionStateSync.SyncAsync(
                uow, profile, section, request.OfficerId, DateTime.UtcNow, ct);
        }

        await uow.SaveChangesAsync(ct);

        // ===== Reviews =====
        var reviewRepo = uow.GetEntityRepository<ReviewItem>();
        var reviewItems = await reviewRepo.DbSet
            .AsNoTracking()
            .Where(r =>
                r.UserProfileId == profile.Id &&
                r.ProfileChangeId == null &&
                !r.IsDeleted)
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
                    HasAttachments = itemDtosBySection.Any(i => i.TargetType == ReviewTargetType.Attachment),
                    InternalReviewerNote = internalNotesBySection.GetValueOrDefault(sec.ToString())
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
            InternalReviewerNote = internalReviewerNote,
            Sections = sections
        };
        var openProfileNote = JsonSerializer.Serialize(new
        {
            eventType = "OpenProfile", message = UserProfileLogConstants.Notes.ProfileOpenedForReview
        });

        await auditRepo.AddAsync(
            new AuditTrailEntry
            {
                UserProfileId = profile.Id,
                UserId = request.OfficerId,
                ActionType = UserProfileLogConstants.ActionTypes.OpenProfile,
                Notes = openProfileNote,
                Section = nameof(ProfileSection.Personal)
            }, ct);
        await loggerRepo.AddAsync(
            new UserProfileLogger
            {
                UserProfileId = profile.Id,
                PerformedById = request.OfficerId,
                ActionType = UserProfileLogConstants.ActionTypes.OpenProfile,
                Notes = openProfileNote,
                Section = nameof(ProfileSection.Personal)
            }, ct);
        await uow.SaveChangesAsync(ct);

        return Result.Ok(dto);

        ReviewStatus ResolveStatus(IReadOnlyList<ProfileApprovalItemDto> items, ProfileApprovalItemDto? sectionReview)
        {
            var source = items.Any() ? items : sectionReview != null ? [sectionReview] : [];

            if (source.Any(i => i.Status == ReviewStatus.NeedsCorrection)) return ReviewStatus.NeedsCorrection;
            if (source.Any(i => i.Status == ReviewStatus.Rejected)) return ReviewStatus.Rejected;
            if (source.Any(i => i.Status is ReviewStatus.Pending or ReviewStatus.NotReviewed or ReviewStatus.Solved))
                return ReviewStatus.Pending;

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

        static string? ReadInternalReviewerNote(string? payload)
        {
            if (string.IsNullOrWhiteSpace(payload))
                return null;

            try
            {
                using var document = JsonDocument.Parse(payload);
                return document.RootElement.TryGetProperty("internalReviewerNote", out var note) &&
                       note.ValueKind == JsonValueKind.String
                    ? note.GetString()
                    : null;
            }
            catch (JsonException)
            {
                return null;
            }
        }

        static string? ReadSectionInternalNote(string? payload)
        {
            if (string.IsNullOrWhiteSpace(payload))
                return null;

            try
            {
                using var document = JsonDocument.Parse(payload);
                return document.RootElement.TryGetProperty("note", out var note) &&
                       note.ValueKind == JsonValueKind.String
                    ? note.GetString()
                    : null;
            }
            catch (JsonException)
            {
                return null;
            }
        }
    }
}
