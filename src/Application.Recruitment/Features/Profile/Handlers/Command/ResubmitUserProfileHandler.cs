using Application.Recruitment.Features.Profile.Command;
using Application.Recruitment.Features.Profile.Handlers.Command.SaveOperation;
using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Services;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Recruitment.Features.Profile.Handlers.Command;

public sealed class ResubmitUserProfileHandler(IUnitOfWork uow)
    : IRequestHandler<ResubmitUserProfileCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(ResubmitUserProfileCommand cmd, CancellationToken ct)
    {
        var profile = await UserProfileLoader.GetFullProfileByUserId(uow, cmd.UserId, true, ct);
        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);

        if (profile.Status != UserProfileStatus.RequiresUpdate)
            return Result.Fail<Unit>(ErrorsCodes.ProfileLockedUnderReview);

        if (!profile.IsCompleted())
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotCompleted);

        var reviewRepo      = uow.GetEntityRepository<ReviewItem>();
        var assignmentRepo  = uow.GetEntityRepository<ProfileAssignment>();
        var loggerRepo      = uow.GetEntityRepository<UserProfileLogger>();

        await ProfileReviewItemSync.EnsurePrerequisiteAttachmentItemsAsync(uow, profile, ct);

        // -----------------------------------------
        // 1) Deactivate assignments (same behavior)
        // -----------------------------------------
        var activeAssignments = await assignmentRepo.DbSet
            .Where(a => a.UserProfileId == profile.Id && a.IsActive)
            .ToListAsync(ct);

        foreach (var assignment in activeAssignments)
        {
            assignment.Deactivate();

            var unassignNote = JsonSerializer.Serialize(new
            {
                eventType = "AssignmentReassigned",
                newAssignedUserId = (Guid?)null,
                newAssignedUserName = (string?)null,
                message = UserProfileLogConstants.Notes.ProfileResubmittedToDistribution
            });

            await loggerRepo.AddAsync(new UserProfileLogger
            {
                UserProfileId  = profile.Id,
                PerformedById  = cmd.UserId,
                ActionType     = UserProfileLogConstants.ActionTypes.ProfileUnassigned,
                Notes          = unassignNote,
                Section        = UserProfileLogConstants.Sections.Assignment,
                EntityId       = assignment.Id
            }, ct);
        }

        // ---------------------------------------------------------
        // 2) RESUBMIT RULE: DO NOT CREATE new review items
        //    Only reopen "Solved" items to "Pending" when changed
        // ---------------------------------------------------------
        var items = await reviewRepo.DbSet
            .Where(x => x.UserProfileId == profile.Id)
            .ToListAsync(ct);

        var correctedSections = items
            .Where(item => item.Status == ReviewStatus.Solved)
            .Select(item => item.Section)
            .ToHashSet();

        foreach (var item in items)
        {
            var previousHash = item.CurrentHash;

            var currentValue = ReviewItemSnapshotBuilder.GetCurrentValue(profile, item);
            item.UpdateHash(currentValue);

            // Important: update resource ID for attachments so they remain findable by the UI
            if (item.TargetType == ReviewTargetType.Attachment)
            {
                item.ResourceId = ReviewItemSnapshotBuilder.GetAttachmentResourceId(profile, item);
            }

            var valueChanged = !string.Equals(previousHash, item.CurrentHash, StringComparison.Ordinal);

            if (item.TargetType == ReviewTargetType.Attachment)
            {
                var currentResourceId = ReviewItemSnapshotBuilder.GetAttachmentResourceId(profile, item);
                if (currentResourceId.HasValue)
                {
                    item.ResourceId = currentResourceId;
                }
            }

            // Required: convert Solved -> Pending (so it shows up for re-review after fixing correction)
            if (item.Status == ReviewStatus.Solved)
            {
                Reopen(item);
                continue;
            }

            if (item.TargetType == ReviewTargetType.Section &&
                item.Status is ReviewStatus.NeedsCorrection or ReviewStatus.Rejected &&
                correctedSections.Contains(item.Section))
            {
                Reopen(item);
                continue;
            }

            if (!valueChanged)
                continue;

            // ... if it was approved and changed, move to Pending if needed (usually handled by IsOutdated)
            // But for initial resubmit flow, we just mark as Outdated and reopen if it was solved.
        }

        // ---------------------------------------------------------
        // 3) Submit again
        // ---------------------------------------------------------
        profile.Status = UserProfileStatus.Submitted;
        await uow.SaveChangesAsync(ct);

        return Result.Ok(Unit.Value);
    }

    private static void Reopen(ReviewItem item)
    {
        item.Status = ReviewStatus.Pending;
        item.IsOutdated = true;

        // Clear review metadata because we reopened
        item.ReviewedAtUtc = null;
        item.ReviewedById  = null;
        // Keep ReviewerNote so the reviewer remembers why they requested changes
    }
}



