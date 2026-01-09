using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Recruitment.Profile.Command.RevisionOperation;
using Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command.SaveOperation;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command.RevisionOperation;

public sealed class ReviseProfileTrainingDeleteHandler(IUnitOfWork uow) :
    ICommandHandler<ReviseProfileTrainingDeleteCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(ReviseProfileTrainingDeleteCommand cmd, CancellationToken ct)
    {
        var profile = await UserProfileLoader.GetFullProfileByUserId(uow, cmd.UserId, true, ct);
        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);
        
        if(profile.Status != UserProfileStatus.RequiresUpdate)
            return Result.Fail<Unit>(ErrorsCodes.ProfileLockedUnderReview);

        var repo = uow.GetEntityRepository<TrainingCourse>();
        var target = await repo.DbSet
            .FirstOrDefaultAsync(x => x.Id == cmd.Id && x.UserProfileId == profile.Id, ct);

        if (target is null)
            return Result.Fail<Unit>(ErrorsCodes.TrainingNotFound);

        await repo.DeleteAsync(target);
        await ReviewItemSaveHelper.UpdateSectionStatusAsync(uow, profile, ProfileSection.TrainingCourses, ct);
        await uow.SaveChangesAsync(ct);

        return Result.Ok(Unit.Value);
    }
}
