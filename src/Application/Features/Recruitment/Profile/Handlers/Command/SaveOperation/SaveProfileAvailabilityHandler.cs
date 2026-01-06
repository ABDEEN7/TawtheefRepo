using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Recruitment.Profile.Command.SaveOperation;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command.SaveOperation;

public sealed class SaveProfileAvailabilityHandler(
    IUnitOfWork uow
) : ICommandHandler<SaveProfileAvailabilityCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(SaveProfileAvailabilityCommand cmd, CancellationToken ct)
    {
        var profile = await UserProfileLoader.GetFullProfileByUserId(uow, cmd.UserId, true, ct);
        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);

        profile.AvailableForRecruitment = cmd.Request.AvailableForRecruitment;

        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
    }
}
