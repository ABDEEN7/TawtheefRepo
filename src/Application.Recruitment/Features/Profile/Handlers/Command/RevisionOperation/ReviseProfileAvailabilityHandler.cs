using Application.Recruitment.Features.Profile.Command.RevisionOperation;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

namespace Application.Recruitment.Features.Profile.Handlers.Command.RevisionOperation;

public sealed class ReviseProfileAvailabilityHandler(
    IUnitOfWork uow
) : ICommandHandler<ReviseProfileAvailabilityCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(ReviseProfileAvailabilityCommand cmd, CancellationToken ct)
    {
        var profile = await UserProfileLoader.GetFullProfileByUserId(uow, cmd.UserId, true, ct);
        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);

        if (profile.Status != UserProfileStatus.RequiresUpdate)
            return Result.Fail<Unit>(ErrorsCodes.ProfileLockedUnderReview);

        profile.AvailableForRecruitment = cmd.Request.AvailableForRecruitment;

        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
    }
}
