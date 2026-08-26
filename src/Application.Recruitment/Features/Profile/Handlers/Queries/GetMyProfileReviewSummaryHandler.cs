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
            .Where(x => x.UserProfileId == profile.Id)
            .ToListAsync(ct);

        var visibleItems = items
            .Where(x => x.Status != ReviewStatus.Solved)
            .ToList();

        var changed = items.Where(IsUserChanged).ToList();
        var correctedItems = items
            .Where(IsUserCorrectedActionableItem)
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

        var lastUserChangeAt = items
            .Where(i => i.IsOutdated)
            .Select(i => i.UpdatedDate)
            .Where(d => d.HasValue)
            .DefaultIfEmpty()
            .Max();
        if (lastUserChangeAt is null)
        {
            lastUserChangeAt = items
                .Where(i => i.Status == ReviewStatus.Solved)
                .Select(i => i.UpdatedDate)
                .Where(d => d.HasValue)
                .DefaultIfEmpty()
                .Max();
        }

        var canResubmit = profile.Status == UserProfileStatus.RequiresUpdate &&
                          changed.Count > 0 &&
                          !items.Any(IsOutstandingActionableCorrection) &&
                          profile.IsCompleted();
        
        if (profile.Status is not UserProfileStatus.InCreation && 
            profile.Status is not UserProfileStatus.RequiresUpdate &&
            profile.Status is not UserProfileStatus.Submitted)
            return Result.Ok(new MyProfileReviewSummaryDto()
            {
                UserProfileId = profile.Id,
                ProfileStatus = profile.Status,
                HasSavedChanges = changed.Count > 0,
                ChangedSections = displayChangedSections,
                ChangedItems = correctedItems,
                LastUserChangeAtUtc = lastUserChangeAt,
                CanResubmit = canResubmit
            });
        

        var notes = visibleItems
            .Where(IsReviewerNote)
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

                var secPendingCount = visibleItems.Count(i => i.Section == sec && i.IsOutdated);
                return new MyProfileReviewSectionDto
                {
                    Section = sec,
                    NotesCount = secNotes.Count,
                    Notes = secNotes
                        .OrderByDescending(n => n.ReviewedAtUtc)
                        .ToArray(),
                    HasUserChanges = secPendingCount > 0,
                    PendingItemsCount = secPendingCount
                };
            })
            .ToArray();

        var dto = new MyProfileReviewSummaryDto {
            UserProfileId = profile.Id,
            ProfileStatus = profile.Status,
            Sections = sections,
            TotalNotes = notes.Count,
            HasSavedChanges = changed.Count > 0,
            ChangedSections = displayChangedSections,
            ChangedItems = correctedItems,
            LastReviewerActionAtUtc = lastReviewerAt,
            LastUserChangeAtUtc = lastUserChangeAt,
            CanResubmit = canResubmit
        };

        return Result.Ok(dto);

        bool IsReviewerNote(ReviewItem x) =>
            x.Status is ReviewStatus.NeedsCorrection or ReviewStatus.Rejected;

        bool IsUserChanged(ReviewItem x) => x.IsOutdated || x.Status == ReviewStatus.Solved;

        bool IsUserCorrectedActionableItem(ReviewItem x) =>
            x.Status == ReviewStatus.Solved &&
            (x.TargetType != ReviewTargetType.Section ||
             x.Section is ProfileSection.Skills or ProfileSection.Languages);

        bool IsOutstandingActionableCorrection(ReviewItem x) =>
            x.Status is ReviewStatus.NeedsCorrection or ReviewStatus.Rejected &&
            (x.TargetType != ReviewTargetType.Section ||
             x.Section is ProfileSection.Skills or ProfileSection.Languages);
    }
}

