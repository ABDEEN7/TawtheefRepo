using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileApprovals.DTOs;
using Tawtheef.Application.Features.Recruitment.Profile.Command;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command;

public sealed class SubmitUserProfileHandler(
    IUnitOfWork uow
) : IRequestHandler<SubmitUserProfileCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(SubmitUserProfileCommand cmd, CancellationToken ct)
    {
        var profileRepo = uow.GetEntityRepository<UserProfile>();

        var profile = await profileRepo.DbSet
            .Include(p => p.ResidenceAddress)
            .Include(p => p.SponsorProfile)
            .Include(p => p.Qualifications)
            .Include(p => p.Languages)
            .FirstOrDefaultAsync(p => p.UserId == cmd.UserId, ct);

        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);

        if (profile.Status is not UserProfileStatus.InCreation)
            return Result.Fail<Unit>(ErrorsCodes.ProfileLockedUnderReview);

        if (!profile.IsCompleted())
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotCompleted);

        // إعداد عناصر المراجعة على مستوى Section فقط
        var reviewRepo = uow.GetEntityRepository<ReviewItem>();

        // جلب الموجود مسبقًا (إن كان المستخدم أعاد الإرسال بعد طلب تعديلات)
        var existingSectionItems = await reviewRepo.DbSet
            .Where(x => x.UserProfileId == profile.Id &&
                        x.TargetType == ReviewTargetType.Section)
            .ToListAsync(ct);

        foreach (var sec in ProfileApprovalFlow.Sections)
        {
            var item = existingSectionItems.FirstOrDefault(x => x.Section == sec);

            if (item is null)
            {
                item = ReviewItem.Create(profile.Id, section: sec, targetType: ReviewTargetType.Section);
                item.Status = ReviewStatus.Pending;
                item.IsOutdated = true;
                item.ReviewedAtUtc = null;
                item.ReviewedById = null;
                item.ReviewerNote = null;

                await reviewRepo.AddAsync(item);
            }
            else
            {
                // Reset state لكل Section عند إعادة الإرسال
                item.Status = ReviewStatus.Pending;
                item.IsOutdated = true;
                item.ReviewedAtUtc = null;
                item.ReviewedById = null;
                item.ReviewerNote = null;

                // تقييد صارم لمرحلة Full Review: يجب ألا يحمل تفاصيل Field/Row/Attachment
                item.FieldPath = null;
                item.EntityName = null;
                item.EntityId = null;
                item.ResourceId = null;
                item.AttachmentTitle = null;
                item.ProfileChangeId = null;
            }
        }

        profile.Status = UserProfileStatus.Submitted;

        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
    }
}
