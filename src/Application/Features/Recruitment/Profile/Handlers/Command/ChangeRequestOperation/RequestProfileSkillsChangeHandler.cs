using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Validations;
using Tawtheef.Application.Features.Recruitment.Profile.Command.ChangeRequestOperation;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command.ChangeRequestOperation;

public sealed class RequestProfileSkillsChangeHandler(
    IUnitOfWork uow,
    IProfileStepValidationService validationService,
    IProfileReviewService reviewService
) : IRequestHandler<RequestProfileSkillsChangeCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(RequestProfileSkillsChangeCommand cmd, CancellationToken ct)
    {
        var profileRepo = uow.GetEntityRepository<UserProfile>();
        var profile = await profileRepo.DbSet
            .Include(p => p.ResidenceAddress)
            .Include(p => p.Qualifications)
            .Include(p => p.Experiences)
            .Include(p => p.TrainingCourses)
            .Include(p => p.Achievements)
            .Include(p => p.Skills)
            .FirstOrDefaultAsync(p => p.UserId == cmd.UserId, ct);

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
            await reviewService.TouchRowAsync(profile.Id, ProfileSection.Skills, "Skill", Guid.NewGuid(), cmd.UserId, ct, null, skill);
        }

        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
    }
}

