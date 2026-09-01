using Application.Recruitment.Features.Profile.Command.SaveOperation;
using MediatR;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

namespace Application.Recruitment.Features.Profile.Handlers.Command.SaveOperation;

public sealed class SaveProfileAvailabilityHandler(
    IUnitOfWork uow,
    UserManager<User> userManager
) : IRequestHandler<SaveProfileAvailabilityCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(SaveProfileAvailabilityCommand cmd, CancellationToken ct)
    {
        var profile = await UserProfileLoader.GetFullProfileByUserId(uow, cmd.UserId, true, ct);
        if (profile is null)
        {
            var user = await userManager.FindByIdAsync($"{cmd.UserId}");
            if (user is null) return Result.Fail<Unit>(ErrorsCodes.UserNotFound);
            var logins = await userManager.GetLoginsAsync(user);
            var providerName = logins.FirstOrDefault()?.ProviderDisplayName?.Replace(" ", "") ?? "Unknown";
            profile = new UserProfile
            {
                UserId = cmd.UserId,
                Provider = providerName,
                Status = UserProfileStatus.InCreation,
                AvailableForRecruitment = cmd.Request.AvailableForRecruitment
            };
            await uow.GetEntityRepository<UserProfile>().AddAsync(profile, ct);
        }
        else
        {
            profile.AvailableForRecruitment = cmd.Request.AvailableForRecruitment;
            await uow.SaveChangesAsync(ct);
        }

        return Result.Ok(Unit.Value);
    }
}
