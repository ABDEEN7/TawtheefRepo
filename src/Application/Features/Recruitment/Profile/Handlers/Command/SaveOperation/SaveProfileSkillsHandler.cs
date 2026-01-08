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
        var skillRepo = uow.GetEntityRepository<ProfileSkill>();

        var profile = await UserProfileLoader.GetFullProfileByUserId(uow, cmd.UserId, true, ct);
        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);

        if (profile.Status is not UserProfileStatus.InCreation &&
            profile.Status is not UserProfileStatus.RequiresUpdate)
            return Result.Fail<Unit>(ErrorsCodes.ProfileLockedUnderReview);

        var validationResult = validationService.ValidateSkills(profile);
        if (validationResult.IsFailed)
            return Result.Fail<Unit>(validationResult.Errors);

        // If user cleared the list in edit mode => remove all existing skills.
        if (cmd.Request.Skills is null || cmd.Request.Skills.Count == 0)
        {
            if (profile.Skills is { Count: > 0 })
                skillRepo.DbSet.RemoveRange(profile.Skills);

            await ReviewItemSaveHelper.UpdateSectionStatusAsync(uow, profile, ProfileSection.Skills, ct);
            await uow.SaveChangesAsync(ct);
            return Result.Ok(Unit.Value);
        }

        // Normalize incoming list (avoid duplicates by SkillId).
        // If duplicates exist, keep the last occurrence (or you can Fail instead).
        var incomingBySkillId = cmd.Request.Skills
            .GroupBy(x => x.SkillId)
            .ToDictionary(g => g.Key, g => g.Last());

        // Existing skills indexed by SkillId
        var existing = profile.Skills ?? new List<ProfileSkill>();
        var existingBySkillId = existing.ToDictionary(x => x.SkillId);

        // 1) Update existing + Add missing
        foreach (var (skillId, incoming) in incomingBySkillId)
        {
            if (existingBySkillId.TryGetValue(skillId, out var row))
            {
                // Update editable fields
                if (row.LevelId != incoming.LevelId)
                    row.LevelId = incoming.LevelId;
            }
            else
            {
                await skillRepo.AddAsync(new ProfileSkill
                {
                    UserProfileId = profile.Id,
                    SkillId = incoming.SkillId,
                    LevelId = incoming.LevelId
                });
            }
        }

        // 2) Remove skills that user deleted in edit mode
        var toRemove = existing
            .Where(x => !incomingBySkillId.ContainsKey(x.SkillId))
            .ToList();

        if (toRemove.Count > 0)
            skillRepo.DbSet.RemoveRange(toRemove);

        await ReviewItemSaveHelper.UpdateSectionStatusAsync(uow, profile, ProfileSection.Skills, ct);
        await uow.SaveChangesAsync(ct);

        return Result.Ok(Unit.Value);
    }
}
