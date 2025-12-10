using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Recruitment.Profile.Command;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command;

public sealed class ReviewProfileItemHandler(IUnitOfWork uow)
    : IRequestHandler<ReviewProfileItemCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(ReviewProfileItemCommand cmd, CancellationToken ct)
    {
        var repo = uow.GetEntityRepository<ReviewItem>();

        var item = await repo.DbSet
            .FirstOrDefaultAsync(r => r.Id == cmd.ReviewItemId, ct);

        if (item is null)
            return Result.Fail<Unit>(ErrorsCodes.ReviewItemNotFound);

        if (cmd.Status == ReviewStatus.NeedsCorrection && string.IsNullOrWhiteSpace(cmd.Note))
            return Result.Fail<Unit>(ErrorsCodes.NotesRequiredForCorrection);

        if (cmd.Status == ReviewStatus.Approved && item.TargetType == ReviewTargetType.Section)
        {
            var hasUnapprovedAttachments = await repo.DbSet
                .AnyAsync(r => r.UserProfileId == item.UserProfileId
                               && r.Section == item.Section
                               && r.TargetType == ReviewTargetType.Attachment
                               && r.Status != ReviewStatus.Approved, ct);

            if (hasUnapprovedAttachments)
                return Result.Fail<Unit>(ErrorsCodes.SectionHasUnapprovedAttachments);
        }

        switch (cmd.Status)
        {
            case ReviewStatus.Approved:
                item.Approve(cmd.ReviewerId, cmd.Note);
                item.ApprovedAtVersion = item.Version;
                break;
            case ReviewStatus.Rejected:
                item.Reject(cmd.ReviewerId, cmd.Note ?? string.Empty);
                break;
            case ReviewStatus.NeedsCorrection:
                item.RequestChanges(cmd.ReviewerId, cmd.Note ?? string.Empty);
                if (item.TargetType == ReviewTargetType.Attachment)
                {
                    var sectionItem = await repo.DbSet
                        .Where(r => r.UserProfileId == item.UserProfileId
                                    && r.Section == item.Section
                                    && r.TargetType == ReviewTargetType.Section)
                        .OrderByDescending(r => r.Version)
                        .FirstOrDefaultAsync(ct);

                    sectionItem?.RequestChanges(cmd.ReviewerId, cmd.Note ?? string.Empty);
                }
                break;
            default:
                item.Status = ReviewStatus.Pending;
                item.ReviewerNote = cmd.Note;
                item.ReviewedById = cmd.ReviewerId;
                item.ReviewedAtUtc = DateTime.UtcNow;
                break;
        }

        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
    }
}
