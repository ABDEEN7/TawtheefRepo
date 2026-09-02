using Application.Recruitment.Features.Profile.DTOs;
using Application.Recruitment.Features.Profile.Queries;
using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Recruitment.Features.Profile.Handlers.Queries;


public sealed class GetMyProfileReviewSummaryHandler(IUnitOfWork uow)
    : IRequestHandler<GetMyProfileReviewSummaryQuery, IResult<MyProfileReviewSummaryDto>>
{
    public async Task<IResult<MyProfileReviewSummaryDto>> Handle(
        GetMyProfileReviewSummaryQuery request, CancellationToken ct)
    {
        var profile = await UserProfileLoader.GetFullProfileByUserId(uow, request.UserId, ct: ct);

        if (profile is null)
            return Result.Fail<MyProfileReviewSummaryDto>(ErrorsCodes.ProfileNotFound);
        

        var items = await uow.GetEntityRepository<ReviewItem>()
            .DbSet.AsNoTracking()
            .Where(x =>
                x.UserProfileId == profile.Id &&
                x.ProfileChangeId == null &&
                !x.IsDeleted)
            .ToListAsync(ct);

        var activeItems = items
            .Where(item => item.IsActiveInCurrentProfile(profile))
            .ToList();

        var outstandingItems = activeItems
            .Where(item => item.IsOutstandingCandidateCorrection())
            .ToList();

        var visibleItems = activeItems
            .Where(item => item.Status != ReviewStatus.Solved)
            .ToList();

        var correctedReviewItems = activeItems
            .Where(item => item.IsCandidateCorrectedItem())
            .ToList();
        var correctedItems = correctedReviewItems
            .OrderBy(x => (int)x.Section)
            .ThenBy(x => (int)x.TargetType)
            .ThenBy(x => x.AttachmentTitle ?? x.FieldPath ?? x.EntityName ?? string.Empty)
            .Select(x => new MyProfileReviewChangedItemDto
            {
                ReviewItemId = x.Id,
                Section = x.Section,
                TargetType = x.TargetType,
                Title = x.AttachmentTitle ?? x.FieldPath ?? x.EntityName ?? "?",
                Note = x.ReviewerNote,
                FieldPath = x.FieldPath,
                EntityId = x.EntityId,
                EntityName = x.EntityName,
                ResourceId = x.ResourceId
            })
            .ToList();
        var displayChangedSections = correctedItems
            .Select(x => x.Section)
            .Distinct()
            .OrderBy(x => (int)x)
            .ToList();

        var lastUserChangeAt = correctedReviewItems
            .Select(i => i.UpdatedDate)
            .Where(d => d.HasValue)
            .DefaultIfEmpty()
            .Max();

        var canResubmit = profile.Status == UserProfileStatus.RequiresUpdate &&
                          correctedReviewItems.Count > 0 &&
                          !outstandingItems.Any() &&
                          profile.IsCompleted();
        
        if (profile.Status is not UserProfileStatus.InCreation && 
            profile.Status is not UserProfileStatus.RequiresUpdate &&
            profile.Status is not UserProfileStatus.Submitted)
            return Result.Ok(new MyProfileReviewSummaryDto()
            {
                UserProfileId = profile.Id,
                ProfileStatus = profile.Status,
                HasSavedChanges = correctedReviewItems.Count > 0,
                ChangedSections = displayChangedSections,
                ChangedItems = correctedItems,
                LastUserChangeAtUtc = lastUserChangeAt,
                CanResubmit = canResubmit
            });
        

        var noteItems = activeItems
            .Where(item =>
                item.IsCandidateActionableTarget() &&
                !string.IsNullOrWhiteSpace(item.ReviewerNote))
            .ToList();

        var notes = noteItems
            .Select(x => new MyProfileReviewNoteDto {
                ReviewItemId = x.Id,
                TargetType   = x.TargetType,
                Status       = x.Status,
                Title        = x.AttachmentTitle ?? x.FieldPath ?? x.EntityName ?? "?",
                Note         = x.ReviewerNote,
                FieldPath    = x.FieldPath,
                EntityId     = x.EntityId,
                EntityName   = x.EntityName,
                ResourceId   = x.ResourceId,
                ReviewedAtUtc = x.ReviewedAtUtc
            })
            .ToList();

        var notesBySection = notes
            .GroupBy(n => items.First(i => i.Id == n.ReviewItemId).Section)
            .ToDictionary(g => g.Key, g => g.ToList());
        
        var lastReviewerAt = visibleItems
            .Where(i => i.ReviewedAtUtc != null)
            .Select(i => i.ReviewedAtUtc)
            .DefaultIfEmpty()
            .Max();

        var sections = Enum.GetValues<ProfileSection>()
            .Select(sec => {
                var secNotes = notesBySection.TryGetValue(sec, out var list) ? list : [];

                var secPendingCount = outstandingItems.Count(i => i.Section == sec);
                var hasActionableSectionData = outstandingItems.Any(item =>
                    item.Section == sec &&
                    item.TargetType == ReviewTargetType.Field &&
                    item.FieldPath == ProfileReviewConstants.FieldPaths.SectionData);
                return new MyProfileReviewSectionDto
                {
                    Section = sec,
                    NotesCount = secNotes.Count,
                    Notes = secNotes
                        .OrderByDescending(n => n.ReviewedAtUtc)
                        .ToArray(),
                    HasActionableSectionData = hasActionableSectionData,
                    HasUserChanges = correctedReviewItems.Any(item => item.Section == sec),
                    PendingItemsCount = secPendingCount
                };
            })
            .ToArray();

        var dto = new MyProfileReviewSummaryDto {
            UserProfileId = profile.Id,
            ProfileStatus = profile.Status,
            Sections = sections,
            TotalNotes = notes.Count,
            HasSavedChanges = correctedReviewItems.Count > 0,
            ChangedSections = displayChangedSections,
            ChangedItems = correctedItems,
            LastReviewerActionAtUtc = lastReviewerAt,
            LastUserChangeAtUtc = lastUserChangeAt,
            CanResubmit = canResubmit
        };

        return Result.Ok(dto);

    }
}

