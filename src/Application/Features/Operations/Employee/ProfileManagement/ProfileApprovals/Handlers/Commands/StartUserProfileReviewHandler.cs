using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileApprovals.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileApprovals.Handlers.Commands;

public sealed class StartUserProfileReviewHandler(IUnitOfWork uow)
    : IRequestHandler<StartUserProfileReviewCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(StartUserProfileReviewCommand cmd, CancellationToken ct)
    {
        var profile = await uow.GetEntityRepository<UserProfile>().DbSet
            .FirstOrDefaultAsync(p => p.Id == cmd.UserProfileId, ct);

        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);

        if (profile.Status != UserProfileStatus.Submitted)
            return Result.Fail<Unit>(ErrorsCodes.NotSubmitted);

        profile.Status = UserProfileStatus.UnderReview;
        await uow.SaveChangesAsync(ct);

        return Result.Ok(Unit.Value);
    }
}
