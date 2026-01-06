using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileApprovals.Commands;
using Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileApprovals.DTOs;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileApprovals.Handlers.Commands;

public sealed class FinalizeUserProfileReviewHandler(IUnitOfWork uow)
    : ICommandHandler<FinalizeUserProfileReviewCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(FinalizeUserProfileReviewCommand cmd, CancellationToken ct)
    {
        var profileRepo = uow.GetEntityRepository<UserProfile>();
        var auditRepo = uow.GetEntityRepository<AuditTrailEntry>();
        var loggerRepo = uow.GetEntityRepository<UserProfileLogger>();
        var assignmentRepo = uow.GetEntityRepository<ProfileAssignment>();
        
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
        profile.Status = hasCorrections ? UserProfileStatus.RequiresUpdate: UserProfileStatus.Approved;

        await auditRepo.AddAsync(new AuditTrailEntry
        {
            UserProfileId = profile.Id,
            UserId = cmd.OfficerId,
            ActionType = "ProfileReviewFinalized",
            Notes = hasCorrections
                ? "Profile review finalized with corrections requested"
                : "Profile review finalized as approved",
            Section = nameof(ProfileSection.Personal)
        });

        await loggerRepo.AddAsync(new UserProfileLogger
        {
            UserProfileId = profile.Id,
            PerformedById = cmd.OfficerId,
            ActionType = "ProfileReviewFinalized",
            Notes = hasCorrections
                ? "Profile review finalized with corrections requested"
                : "Profile review finalized as approved",
            Section = nameof(ProfileSection.Personal),
            ReviewStatus = hasCorrections ? ReviewStatus.NeedsCorrection : ReviewStatus.Approved
        });

        var activeAssignments = await assignmentRepo.DbSet
            .Where(a => a.UserProfileId == profile.Id && a.IsActive)
            .ToListAsync(ct);

        foreach (var assignment in activeAssignments)
        {
            assignment.Deactivate();

            await loggerRepo.AddAsync(new UserProfileLogger
            {
                UserProfileId = assignment.UserProfileId,
                PerformedById = cmd.OfficerId,
                ActionType = UserProfileLogConstants.ActionTypes.ProfileUnassigned,
                Notes = "Assignment closed when review finalized",
                Section = "Assignment",
                EntityId = assignment.Id
            });
        }

        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
    }
}
