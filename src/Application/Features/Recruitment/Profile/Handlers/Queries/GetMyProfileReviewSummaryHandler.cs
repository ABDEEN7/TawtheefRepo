using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Recruitment.Profile.DTOs;
using Tawtheef.Application.Features.Recruitment.Profile.Queries;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Queries;


public sealed class GetMyProfileReviewSummaryHandler(IUnitOfWork uow)
    : IRequestHandler<GetMyProfileReviewSummaryQuery, IResult<MyProfileReviewSummaryDto>>
{
    public async Task<IResult<MyProfileReviewSummaryDto>> Handle(
        GetMyProfileReviewSummaryQuery request, CancellationToken ct)
    {
        var profile = await uow.GetEntityRepository<UserProfile>()
            .DbSet.AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == request.UserId, ct);

        if (profile is null)
            return Result.Fail<MyProfileReviewSummaryDto>(ErrorsCodes.ProfileNotFound);
        

        var items = await uow.GetEntityRepository<ReviewItem>()
            .DbSet.AsNoTracking()
            .Where(x => x.UserProfileId == profile.Id)
            .ToListAsync(ct);

        var changed = items.Where(IsUserChanged).ToList();
        var changedSections = changed
            .Select(x => x.Section)
            .Distinct()
            .OrderBy(x => (int)x)
            .ToList();

        var lastUserChangeAt = items
            .Where(i => i.IsOutdated)
            .Max(i => i.UpdatedDate);
        
        if(profile.Status is not UserProfileStatus.InCreation)
            return Result.Ok(new MyProfileReviewSummaryDto()
            {
                UserProfileId = profile.Id,
                ProfileStatus = profile.Status,
                HasSavedChanges = changed.Count > 0,
                ChangedSections = changedSections,
                LastUserChangeAtUtc = lastUserChangeAt,
                CanResubmit = profile.Status == UserProfileStatus.RequiresUpdate && changed.Count > 0
            });
        

        var notes = items
            .Where(IsReviewerNote)
            .Select(x => new MyProfileReviewNoteDto {
                ReviewItemId = x.Id,
                TargetType   = x.TargetType,
                Status       = x.Status,
                Title        = x.AttachmentTitle ?? x.FieldPath ?? x.EntityName ?? "—",
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
        
        var lastReviewerAt = items
            .Where(i => i.ReviewedAtUtc != null)
            .Max(i => i.ReviewedAtUtc);

        var sections = Enum.GetValues<ProfileSection>()
            .Select(sec => {
                var secNotes = notesBySection.TryGetValue(sec, out var list) ? list : [];

                var secPendingCount = items.Count(i => i.Section == sec && i.IsOutdated);
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
            ChangedSections = changedSections,
            LastReviewerActionAtUtc = lastReviewerAt,
            LastUserChangeAtUtc = lastUserChangeAt,
            CanResubmit = profile.Status == UserProfileStatus.RequiresUpdate && changed.Count > 0
        };

        return Result.Ok(dto);

        bool IsReviewerNote(ReviewItem x) =>
            x is { ReviewedAtUtc: not null, Status: ReviewStatus.NeedsCorrection or ReviewStatus.Rejected };

        bool IsUserChanged(ReviewItem x) => x.IsOutdated;
    }
}
