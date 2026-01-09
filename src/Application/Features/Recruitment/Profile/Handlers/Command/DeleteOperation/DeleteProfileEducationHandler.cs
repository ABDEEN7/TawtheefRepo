using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Recruitment.Profile.Command.DeleteOperation;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command.DeleteOperation;

public sealed class DeleteProfileEducationHandler(IUnitOfWork uow) :
    ICommandHandler<DeleteProfileEducationCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(DeleteProfileEducationCommand cmd, CancellationToken ct)
    {
        var profile = await UserProfileLoader.GetFullProfileByUserId(uow, cmd.UserId, false, ct);
        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);
        
        if(profile.Status != UserProfileStatus.InCreation)
            return Result.Fail<Unit>(ErrorsCodes.ProfileLockedUnderReview);

        var qualificationRepo = uow.GetEntityRepository<Qualification>();
        var target = await qualificationRepo.DbSet
            .FirstOrDefaultAsync(x => x.Id == cmd.Id && x.UserProfileId == profile.Id, ct);

        if (target is null)
            return Result.Fail<Unit>(ErrorsCodes.DegreeNotFound);

        var experiencesRepo = uow.GetEntityRepository<Experience>();
        var isLinkedToExperience = await experiencesRepo.DbSet
            .AnyAsync(x => x.QualificationId == target.Id && x.UserProfileId == profile.Id, ct);

        if (isLinkedToExperience)
            return Result.Fail<Unit>(ErrorsCodes.DegreeLinkedToExperience);

        await qualificationRepo.DeleteAsync(target);
        await uow.SaveChangesAsync(ct);

        return Result.Ok(Unit.Value);
    }
}
