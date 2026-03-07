using Application.Recruitment.Features.Profile.Command.DeleteOperation;
using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Users;

namespace Application.Recruitment.Features.Profile.Handlers.Command.DeleteOperation;

public sealed class DeleteProfileSkillHandler(IUnitOfWork uow) :
    IRequestHandler<DeleteProfileSkillCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(DeleteProfileSkillCommand cmd, CancellationToken ct)
    {
        var profile = await UserProfileLoader.GetFullProfileByUserId(uow, cmd.UserId, false, ct);
        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);
        
        if(profile.Status != UserProfileStatus.InCreation)
            return Result.Fail<Unit>(ErrorsCodes.ProfileLockedUnderReview);

        var repo = uow.GetEntityRepository<ProfileSkill>();
        var target = await repo.DbSet
            .FirstOrDefaultAsync(x => x.Id == cmd.Id && x.UserProfileId == profile.Id, ct);

        if (target is null)
            return Result.Fail<Unit>(ErrorsCodes.SkillNotFound);

        await repo.DeleteAsync(target);
        await uow.SaveChangesAsync(ct);

        return Result.Ok(Unit.Value);
    }
}

