using Application.Recruitment.Features.Profile.Command.RevisionOperation;
using Application.Recruitment.Features.Profile.Handlers.Command.SaveOperation;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Validations;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Recruitment.Features.Profile.Handlers.Command.RevisionOperation.Save;


public sealed class ReviseProfileLanguagesHandler(
    IUnitOfWork uow,
    IProfileStepValidationService validationService
) : ICommandHandler<ReviseProfileLanguagesCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(ReviseProfileLanguagesCommand cmd, CancellationToken ct)
    {
        var langRepo = uow.GetEntityRepository<ProfileLanguage>();

        var profile = await UserProfileLoader.GetFullProfileByUserId(uow, cmd.UserId, true, ct);
        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);

        if (profile.Status != UserProfileStatus.RequiresUpdate)
            return Result.Fail<Unit>(ErrorsCodes.ProfileLockedUnderReview);

        var validationResult = validationService.ValidateLanguages(profile);
        if (validationResult.IsFailed)
            return Result.Fail<Unit>(validationResult.Errors);

        profile.Languages ??= new List<ProfileLanguage>();

        // Clear all
        if (cmd.Request.Languages.Count == 0)
        {
            if (profile.Languages.Count > 0)
                langRepo.DbSet.RemoveRange(profile.Languages);

            await ReviewItemSaveHelper.UpdateSectionStatusAsync(uow, profile, ProfileSection.Languages, ct);
            await uow.SaveChangesAsync(ct);
            return Result.Ok(Unit.Value);
        }

        // Deduplicate incoming by LanguageId (or Fail if duplicates)
        var incomingByLanguageId = cmd.Request.Languages
            .GroupBy(x => x.LanguageId)
            .ToDictionary(g => g.Key, g => g.Last());

        var existing = profile.Languages;
        var existingByLanguageId = existing.ToDictionary(x => x.LanguageId);

        // Upsert
        foreach (var (languageId, dto) in incomingByLanguageId)
        {
            if (existingByLanguageId.TryGetValue(languageId, out var row))
            {
                row.SpeakingLevelId = dto.SpeakingLevelId;
                row.WritingLevelId = dto.WritingLevelId;
                row.ReadingLevelId = dto.ReadingLevelId;
            }
            else
            {
                // If your repo AddAsync doesn't take CT, use DbSet.Add or DbSet.AddAsync(entity, ct)
                await langRepo.DbSet.AddAsync(
                    new ProfileLanguage
                    {
                        UserProfileId = profile.Id,
                        LanguageId = dto.LanguageId,
                        SpeakingLevelId = dto.SpeakingLevelId,
                        WritingLevelId = dto.WritingLevelId,
                        ReadingLevelId = dto.ReadingLevelId
                    }, ct);
            }
        }

        // Delete removed
        var toRemove = existing
            .Where(x => !incomingByLanguageId.ContainsKey(x.LanguageId))
            .ToList();

        if (toRemove.Count > 0)
            langRepo.DbSet.RemoveRange(toRemove);

        await ReviewItemSaveHelper.UpdateSectionStatusAsync(uow, profile, ProfileSection.Languages, ct);
        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
    }
}
