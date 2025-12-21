using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Validations;
using Tawtheef.Application.Features.Recruitment.Profile.Command;
using Tawtheef.Application.Features.Recruitment.Profile.Command.SaveOperations;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command.SaveOperation;


public sealed class SaveProfileLanguagesHandler(
    IUnitOfWork uow,
    IProfileStepValidationService validationService,
    IProfileReviewService reviewService
) : IRequestHandler<SaveProfileLanguagesCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(SaveProfileLanguagesCommand cmd, CancellationToken ct)
    {
        var profileRepo = uow.GetEntityRepository<UserProfile>();
        var langRepo    = uow.GetEntityRepository<ProfileLanguage>();

        var profile = await profileRepo.DbSet
            .Include(p => p.ResidenceAddress)
            .Include(p => p.Qualifications)
            .Include(p => p.Experiences)
            .Include(p => p.TrainingCourses)
            .Include(p => p.Achievements)
            .Include(p => p.Skills)
            .Include(p => p.Languages)
            .FirstOrDefaultAsync(p => p.UserId == cmd.UserId, ct);

        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);

        var validationResult = validationService.ValidateLanguages(profile);
        if (validationResult.IsFailed)
            return Result.Fail<Unit>(validationResult.Errors);

        if(cmd.Request.Languages.Count == 0)
            return Result.Ok(Unit.Value);

        if (profile.Status is not UserProfileStatus.InCreation)
        {
            foreach (var language in cmd.Request.Languages)
            {
                await reviewService.TouchRowAsync(profile.Id, ProfileSection.Languages, "Language", Guid.NewGuid(), cmd.UserId, ct, null, language);
            }
            await uow.SaveChangesAsync(ct);
            return Result.Ok(Unit.Value);
        }
        
        if (profile.Languages is not null && profile.Languages.Count > 0)
        {
            langRepo.DbSet.RemoveRange(profile.Languages);
        }
        var languages = cmd.Request.Languages
            .Select(l => new ProfileLanguage
            {
                LanguageId    = l.LanguageId,
                SpeakingLevelId = l.SpeakingLevelId,
                WritingLevelId  = l.WritingLevelId,
                ReadingLevelId  = l.ReadingLevelId,
                UserProfileId = profile.Id
            }).ToList();

        await langRepo.AddRangeAsync(languages);

        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
    }
}
