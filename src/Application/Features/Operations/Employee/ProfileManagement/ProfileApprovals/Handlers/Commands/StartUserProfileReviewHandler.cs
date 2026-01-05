using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileApprovals.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileApprovals.Handlers.Commands;

public sealed class StartUserProfileReviewHandler(IUnitOfWork uow)
    : IRequestHandler<StartUserProfileReviewCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(StartUserProfileReviewCommand cmd, CancellationToken ct)
    {
        var profileRepo = uow.GetEntityRepository<UserProfile>();
        var auditRepo = uow.GetEntityRepository<AuditTrailEntry>();
        var loggerRepo = uow.GetEntityRepository<UserProfileLogger>();

        var profile = await profileRepo.DbSet
            .FirstOrDefaultAsync(p => p.Id == cmd.UserProfileId, ct);

        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);

        if (profile.Status != UserProfileStatus.Submitted)
            return Result.Fail<Unit>(ErrorsCodes.NotSubmitted);

        profile.Status = UserProfileStatus.UnderReview;

        await auditRepo.AddAsync(new AuditTrailEntry
        {
            UserProfileId = profile.Id,
            UserId = cmd.OfficerId,
            ActionType = "ProfileReviewStarted",
            Notes = "Profile moved to under review",
            Section = nameof(ProfileSection.Personal)
        });

        await loggerRepo.AddAsync(new UserProfileLogger
        {
            UserProfileId = profile.Id,
            PerformedById = cmd.OfficerId,
            ActionType = "ProfileReviewStarted",
            Notes = "Profile moved to under review",
            Section = nameof(ProfileSection.Personal),
            ReviewStatus = ReviewStatus.Pending
        });

        await uow.SaveChangesAsync(ct);

        return Result.Ok(Unit.Value);
    }
}
