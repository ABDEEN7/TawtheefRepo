using Application.Recruitment.Features.Profile.Command.RevisionOperation;
using Application.Recruitment.Features.Profile.Handlers.Command.SaveOperation;
using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Validations;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Recruitment.Features.Profile.Handlers.Command.RevisionOperation.Save;

public sealed class ReviseProfileSkillsHandler(
    IUnitOfWork uow,
    IProfileStepValidationService validationService
) : IRequestHandler<ReviseProfileSkillsCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(ReviseProfileSkillsCommand cmd, CancellationToken ct)
    {
        var skillRepo = uow.GetEntityRepository<ProfileSkill>();
        var reviewRepo = uow.GetEntityRepository<ReviewItem>();

        var profile = await UserProfileLoader.GetFullProfileByUserId(uow, cmd.UserId, true, ct);
        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);

        if (profile.Status != UserProfileStatus.RequiresUpdate)
            return Result.Fail<Unit>(ErrorsCodes.ProfileLockedUnderReview);

        var validationResult = validationService.ValidateSkills(profile);
        if (validationResult.IsFailed)
            return Result.Fail<Unit>(validationResult.Errors);

        var sectionCorrectionIsActionable = await reviewRepo.DbSet.AsNoTracking().AnyAsync(item =>
            item.UserProfileId == profile.Id &&
            item.ProfileChangeId == null &&
            !item.IsDeleted &&
            item.Section == ProfileSection.Skills &&
            item.TargetType == ReviewTargetType.Section &&
            (item.Status == ReviewStatus.NeedsCorrection ||
             item.Status == ReviewStatus.Rejected ||
             (item.Status == ReviewStatus.Solved && item.ReviewedAtUtc != null)), ct);

        if (cmd.Request.Skills.Count == 0)
        {
            if (!sectionCorrectionIsActionable && profile.Skills is { Count: > 0 })
                return Result.Fail<Unit>(ErrorsCodes.InvalidRequest);

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
        if (!sectionCorrectionIsActionable && existing.Any(row =>
                !incomingBySkillId.TryGetValue(row.SkillId, out var incoming) ||
                incoming.LevelId != row.LevelId))
            return Result.Fail<Unit>(ErrorsCodes.InvalidRequest);

        var added = false;

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
                var entity = new ProfileSkill
                {
                    UserProfileId = profile.Id,
                    SkillId = incoming.SkillId,
                    LevelId = incoming.LevelId
                };
                await skillRepo.AddAsync(entity);
                existing.Add(entity);
                added = true;
            }
        }

        // 2) Remove skills that user deleted in edit mode
        var toRemove = existing
            .Where(x => !incomingBySkillId.ContainsKey(x.SkillId))
            .ToList();

        if (toRemove.Count > 0)
            skillRepo.DbSet.RemoveRange(toRemove);

        if (sectionCorrectionIsActionable)
            await ReviewItemSaveHelper.UpdateSectionStatusAsync(uow, profile, ProfileSection.Skills, ct);
        else if (added)
            await ReviewItemSaveHelper.MarkSectionChangedByAdditionAsync(uow, profile, ProfileSection.Skills, ct);
        await uow.SaveChangesAsync(ct);

        return Result.Ok(Unit.Value);
    }
}

