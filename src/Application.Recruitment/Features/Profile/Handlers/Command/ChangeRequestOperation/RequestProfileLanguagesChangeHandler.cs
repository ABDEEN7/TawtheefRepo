using Application.Recruitment.Features.Profile.Command.ChangeRequestOperation;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Validations;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Recruitment.Features.Profile.Handlers.Command.ChangeRequestOperation;

public sealed class RequestProfileLanguagesChangeHandler(
    IUnitOfWork uow,
    IProfileStepValidationService validationService,
    IProfileReviewService reviewService
) : ICommandHandler<RequestProfileLanguagesChangeCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(RequestProfileLanguagesChangeCommand cmd, CancellationToken ct)
    {
        var profile = await UserProfileLoader.GetFullProfileByUserId(uow, cmd.UserId, ct: ct);
        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);

        if (profile.Status == UserProfileStatus.InCreation)
            return Result.Fail<Unit>(ErrorsCodes.NotSubmitted);

        var validationResult = validationService.ValidateLanguages(profile);
        if (validationResult.IsFailed)
            return Result.Fail<Unit>(validationResult.Errors);

        if (cmd.Request.Languages.Count == 0)
            return Result.Ok(Unit.Value);

        foreach (var language in cmd.Request.Languages)
        {
            await reviewService.TouchRowAsync(profile.Id, ProfileSection.Languages, ProfileReviewConstants.EntityNames.Language, Guid.NewGuid(), cmd.UserId, ct, null, language);
        }

        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
    }
}
