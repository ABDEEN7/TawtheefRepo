using Application.Recruitment.Features.Profile.Command.ChangeRequestOperation;
using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Validations;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Recruitment.Features.Profile.Handlers.Command.ChangeRequestOperation;

public sealed class RequestProfileSkillsChangeHandler(
    IUnitOfWork uow,
    IProfileStepValidationService validationService,
    IProfileReviewService reviewService
) : IRequestHandler<RequestProfileSkillsChangeCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(RequestProfileSkillsChangeCommand cmd, CancellationToken ct)
    {
        var profile = await UserProfileLoader.GetFullProfileByUserId(uow, cmd.UserId, ct: ct);
        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);

        if (profile.Status == UserProfileStatus.InCreation)
            return Result.Fail<Unit>(ErrorsCodes.NotSubmitted);

        var validationResult = validationService.ValidateSkills(profile);
        if (validationResult.IsFailed)
            return Result.Fail<Unit>(validationResult.Errors);

        if (cmd.Request.Skills.Count == 0)
            return Result.Ok(Unit.Value);

        foreach (var skill in cmd.Request.Skills)
        {
            await reviewService.TouchRowAsync(profile.Id, ProfileSection.Skills, ProfileReviewConstants.EntityNames.Skill, Guid.NewGuid(), cmd.UserId, ct, null, skill);
        }

        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
    }
}

