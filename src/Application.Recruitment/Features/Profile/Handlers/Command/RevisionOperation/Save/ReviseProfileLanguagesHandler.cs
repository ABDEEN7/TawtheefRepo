using Application.Recruitment.Features.Profile.Command.RevisionOperation;
using Application.Recruitment.Features.Profile.DTOs.SaveOperation;
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

public sealed class ReviseProfileLanguagesHandler(
    IUnitOfWork uow,
    IProfileStepValidationService validationService
) : IRequestHandler<ReviseProfileLanguagesCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(ReviseProfileLanguagesCommand cmd, CancellationToken ct)
    {
        var langRepo = uow.GetEntityRepository<ProfileLanguage>();
        var reviewRepo = uow.GetEntityRepository<ReviewItem>();

        var profile = await UserProfileLoader.GetFullProfileByUserId(uow, cmd.UserId, true, ct);
        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);

        if (profile.Status != UserProfileStatus.RequiresUpdate)
            return Result.Fail<Unit>(ErrorsCodes.ProfileLockedUnderReview);

        var validationResult = validationService.ValidateLanguages(profile);
        if (validationResult.IsFailed)
            return Result.Fail<Unit>(validationResult.Errors);

        profile.Languages ??= new List<ProfileLanguage>();

        var sectionCorrectionIsActionable = await reviewRepo.DbSet.AsNoTracking().AnyAsync(item =>
            item.UserProfileId == profile.Id &&
            item.ProfileChangeId == null &&
            !item.IsDeleted &&
            item.Section == ProfileSection.Languages &&
            item.TargetType == ReviewTargetType.Section &&
            (item.Status == ReviewStatus.NeedsCorrection ||
             item.Status == ReviewStatus.Rejected ||
             (item.Status == ReviewStatus.Solved && item.ReviewedAtUtc != null)), ct);

        // Clear all
        if (cmd.Request.Languages.Count == 0)
        {
            if (!sectionCorrectionIsActionable && profile.Languages.Count > 0)
                return Result.Fail<Unit>(ErrorsCodes.InvalidRequest);

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
        if (!sectionCorrectionIsActionable && existing.Any(row =>
                !incomingByLanguageId.TryGetValue(row.LanguageId, out var incoming) ||
                incoming.SpeakingLevelId != row.SpeakingLevelId ||
                incoming.WritingLevelId != row.WritingLevelId ||
                incoming.ReadingLevelId != row.ReadingLevelId))
            return Result.Fail<Unit>(ErrorsCodes.InvalidRequest);

        var added = false;

        // Upsert
        foreach ((Guid languageId, ProfileLanguageUpsertDto dto) in incomingByLanguageId)
        {
            if (existingByLanguageId.TryGetValue(languageId, out var row))
            {
                row.SpeakingLevelId = dto.SpeakingLevelId;
                row.WritingLevelId = dto.WritingLevelId;
                row.ReadingLevelId = dto.ReadingLevelId;
            }
            else
            {
                var entity = new ProfileLanguage
                {
                    UserProfileId = profile.Id,
                    LanguageId = dto.LanguageId,
                    SpeakingLevelId = dto.SpeakingLevelId,
                    WritingLevelId = dto.WritingLevelId,
                    ReadingLevelId = dto.ReadingLevelId
                };
                await langRepo.DbSet.AddAsync(entity, ct);
                existing.Add(entity);
                added = true;
            }
        }

        // Delete removed
        var toRemove = existing
            .Where(x => !incomingByLanguageId.ContainsKey(x.LanguageId))
            .ToList();

        if (toRemove.Count > 0)
            langRepo.DbSet.RemoveRange(toRemove);

        if (sectionCorrectionIsActionable)
            await ReviewItemSaveHelper.UpdateSectionStatusAsync(uow, profile, ProfileSection.Languages, ct);
        else if (added)
            await ReviewItemSaveHelper.MarkSectionChangedByAdditionAsync(uow, profile, ProfileSection.Languages, ct);
        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
    }
}
