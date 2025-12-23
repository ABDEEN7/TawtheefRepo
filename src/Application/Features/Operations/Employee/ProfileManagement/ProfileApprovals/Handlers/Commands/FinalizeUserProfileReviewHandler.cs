using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileApprovals.Commands;
using Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileApprovals.DTOs;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileApprovals.Handlers.Commands;

public sealed class FinalizeUserProfileReviewHandler(IUnitOfWork uow)
    : IRequestHandler<FinalizeUserProfileReviewCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(FinalizeUserProfileReviewCommand cmd, CancellationToken ct)
    {
        var profileRepo = uow.GetEntityRepository<UserProfile>();
        
        var profile = await profileRepo.DbSet.FirstOrDefaultAsync(p => p.Id == cmd.UserProfileId, ct);
        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);

        if (profile.Status != UserProfileStatus.UnderReview)
            return Result.Fail<Unit>(ErrorsCodes.ProfileNotUnderReview);

        var reviewRepo = uow.GetEntityRepository<ReviewItem>();

        // All sections items should be Approved
        var sectionItems = await reviewRepo.DbSet
            .Where(x => x.UserProfileId == profile.Id &&
                        x.TargetType == ReviewTargetType.Section &&
                        !x.IsDeleted)
            .ToListAsync(ct);

        // Check if all sections exist (some section could be deleted!!)
        var missing = ProfileApprovalFlow.Sections
            .Where(s => sectionItems.All(i => i.Section != s))
            .ToList();

        if (missing.Count > 0)
            return Result.Fail<Unit>(ErrorsCodes.UnapprovedItemsExist);

        // Finalize only if all sections are Approved or NeedsCorrection
        if (sectionItems.Any(i => i.Status == ReviewStatus.Pending))
            return Result.Fail<Unit>(ErrorsCodes.UnapprovedItemsExist);

        var hasCorrections = sectionItems.Any(i => i.Status == ReviewStatus.NeedsCorrection);
        profile.Status = hasCorrections ? UserProfileStatus.InCreation: UserProfileStatus.Approved;

        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
    }
}
