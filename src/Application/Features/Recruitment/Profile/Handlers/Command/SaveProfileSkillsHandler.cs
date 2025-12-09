using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Validations;
using Tawtheef.Application.Features.Recruitment.Profile.Command;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command;


public sealed class SaveProfileSkillsHandler(
    IUnitOfWork uow,
    IProfileReviewService reviewService,
    IProfileStepValidationService validationService
) : IRequestHandler<SaveProfileSkillsCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(SaveProfileSkillsCommand cmd, CancellationToken ct)
    {
        var profileRepo = uow.GetEntityRepository<UserProfile>();
        var skillRepo   = uow.GetEntityRepository<ProfileSkill>();

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
        await reviewService.TouchSectionAsync(profile.Id, ProfileSection.SkillsLanguages, ct);
        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
    }
}
