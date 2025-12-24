using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Validations;
using Tawtheef.Application.Features.Recruitment.Profile.Command.SaveOperation;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command.SaveOperation;


public sealed class SaveProfileLanguagesHandler(
    IUnitOfWork uow,
    IProfileStepValidationService validationService
) : IRequestHandler<SaveProfileLanguagesCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(SaveProfileLanguagesCommand cmd, CancellationToken ct)
    {
        var langRepo    = uow.GetEntityRepository<ProfileLanguage>();

        var profile = await UserProfileLoader.GetFullProfile(uow, cmd.UserId, true, ct);
        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);

        if (profile.Status is not UserProfileStatus.InCreation)
            return Result.Fail<Unit>(ErrorsCodes.ProfileLockedUnderReview);

        var validationResult = validationService.ValidateLanguages(profile);
        if (validationResult.IsFailed)
            return Result.Fail<Unit>(validationResult.Errors);

        if(cmd.Request.Languages.Count == 0)
            return Result.Ok(Unit.Value);
        
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
