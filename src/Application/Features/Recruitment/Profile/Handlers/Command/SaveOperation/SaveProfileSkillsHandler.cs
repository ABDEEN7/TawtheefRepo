using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Validations;
using Tawtheef.Application.Features.Recruitment.Profile.Command.SaveOperation;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command.SaveOperation;


public sealed class SaveProfileSkillsHandler(
    IUnitOfWork uow,
    IProfileStepValidationService validationService
) : ICommandHandler<SaveProfileSkillsCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(SaveProfileSkillsCommand cmd, CancellationToken ct)
    {
        var skillRepo   = uow.GetEntityRepository<ProfileSkill>();

        var profile = await UserProfileLoader.GetFullProfileByUserId(uow, cmd.UserId,true, ct);
        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);

        if (profile.Status is not UserProfileStatus.InCreation && profile.Status is not UserProfileStatus.RequiresUpdate)
            return Result.Fail<Unit>(ErrorsCodes.ProfileLockedUnderReview);

        var validationResult = validationService.ValidateSkills(profile);
        if (validationResult.IsFailed)
            return Result.Fail<Unit>(validationResult.Errors);

        if(cmd.Request.Skills.Count == 0)
        {
            await ReviewItemSaveHelper.UpdateSectionStatusAsync(uow, profile, ProfileSection.Skills, ct);
            await uow.SaveChangesAsync(ct);
            return Result.Ok(Unit.Value);
        }
        
        // Skills
        if (profile.Skills is not null && profile.Skills.Count > 0)
        {
            skillRepo.DbSet.RemoveRange(profile.Skills);
        }

        var skills = cmd.Request.Skills
            .Select(s => new ProfileSkill
            {
                SkillId = s.SkillId,
                LevelId = s.LevelId,
                UserProfileId = profile.Id
            }).ToList();

        await skillRepo.AddRangeAsync(skills);
        
        await ReviewItemSaveHelper.UpdateSectionStatusAsync(uow, profile, ProfileSection.Skills, ct);
        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
    }
}
