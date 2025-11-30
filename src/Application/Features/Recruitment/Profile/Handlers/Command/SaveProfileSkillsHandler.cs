using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Recruitment.Profile.Command;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command;


public sealed class SaveProfileSkillsHandler(
    IUnitOfWork uow
) : IRequestHandler<SaveProfileSkillsCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(SaveProfileSkillsCommand cmd, CancellationToken ct)
    {
        var profileRepo = uow.GetEntityRepository<UserProfile>();
        var skillRepo   = uow.GetEntityRepository<ProfileSkill>();
        var langRepo    = uow.GetEntityRepository<ProfileLanguage>();

        var profile = await profileRepo.DbSet
            .Include(p => p.Skills)
            .Include(p => p.Languages)
            .FirstOrDefaultAsync(p => p.UserId == cmd.UserId, ct);

        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);

        // Skills
        if (profile.Skills is not null && profile.Skills.Count > 0)
        {
            skillRepo.DbSet.RemoveRange(profile.Skills);
        }
        profile.Skills = cmd.Request.Skills
            .Select(s => new ProfileSkill
            {
                SkillId       = s.SkillId,
                UserProfileId = profile.Id
            })
            .ToList();

        // Languages
        if (profile.Languages is not null && profile.Languages.Count > 0)
        {
            langRepo.DbSet.RemoveRange(profile.Languages);
        }
        profile.Languages = cmd.Request.Languages
            .Select(l => new ProfileLanguage
            {
                LanguageId    = l.LanguageId,
                LevelId       = l.LevelId,
                UserProfileId = profile.Id
            })
            .ToList();

        profile.IsDraft = !cmd.Request.Submit;

        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
    }
}
