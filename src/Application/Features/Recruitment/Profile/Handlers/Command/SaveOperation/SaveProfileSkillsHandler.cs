using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Validations;
using Tawtheef.Application.Features.Recruitment.Profile.Command.SaveOperation;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command.SaveOperation;


public sealed class SaveProfileSkillsHandler(
    IUnitOfWork uow,
    IProfileStepValidationService validationService
) : IRequestHandler<SaveProfileSkillsCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(SaveProfileSkillsCommand cmd, CancellationToken ct)
    {
        var skillRepo   = uow.GetEntityRepository<ProfileSkill>();

        var profile = await UserProfileLoader.GetFullProfileByUserId(uow, cmd.UserId,true, ct);
        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);

        if (profile.Status is not UserProfileStatus.InCreation)
            return Result.Fail<Unit>(ErrorsCodes.ProfileLockedUnderReview);

        var validationResult = validationService.ValidateSkills(profile);
        if (validationResult.IsFailed)
            return Result.Fail<Unit>(validationResult.Errors);

        if(cmd.Request.Skills.Count == 0)
            return Result.Ok(Unit.Value);
        
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
        
        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
    }
}
