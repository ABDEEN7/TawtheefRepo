using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Recruitment.Profile.Command;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command;


public sealed class SaveProfileExperienceHandler(
    IUnitOfWork uow
) : IRequestHandler<SaveProfileExperienceCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(SaveProfileExperienceCommand cmd, CancellationToken ct)
    {
        var profileRepo = uow.GetEntityRepository<UserProfile>();
        var expRepo     = uow.GetEntityRepository<Experience>();
        var courseRepo  = uow.GetEntityRepository<TrainingCourse>();

        var profile = await profileRepo.DbSet
            .Include(p => p.Experiences)
            .Include(p => p.TrainingCourses)
            .FirstOrDefaultAsync(p => p.UserId == cmd.UserId, ct);

        if (profile is null)
        {
            profile = new UserProfile
            {
                UserId  = cmd.UserId,
                IsDraft = true
            };
            await profileRepo.AddAsync(profile);
            await uow.SaveChangesAsync(ct);
        }

        // 1) حذف كل الخبرات القديمة
        if (profile.Experiences is not null && profile.Experiences.Count > 0)
        {
            expRepo.DbSet.RemoveRange(profile.Experiences);
        }
        profile.Experiences = new List<Experience>();

        foreach (var e in cmd.Request.Experiences)
        {
            var entity = new Experience
            {
                Organization   = e.Organization,
                Position       = e.Position,
                StartDate      = e.StartDate,
                EndDate        = e.EndDate,
                CertificateId  = e.CertificateId ?? Guid.Empty,
                UserProfileId  = profile.Id,
                Achievements   = (e.Achievements is { Count: > 0 })
                    ? string.Join('\n', e.Achievements)
                    : string.Empty
            };

            profile.Experiences.Add(entity);
        }

        // 2) حذف كل الدورات القديمة
        if (profile.TrainingCourses is not null && profile.TrainingCourses.Count > 0)
        {
            courseRepo.DbSet.RemoveRange(profile.TrainingCourses);
        }
        profile.TrainingCourses = new List<TrainingCourse>();

        foreach (var c in cmd.Request.TrainingCourses)
        {
            var entity = new TrainingCourse
            {
                Organization  = c.Organization,
                Position      = c.Position,
                StartDate     = c.StartDate,
                EndDate       = c.EndDate,
                CertificateId = c.CertificateId ?? Guid.Empty,
                UserProfileId = profile.Id
            };

            profile.TrainingCourses.Add(entity);
        }

        profile.IsDraft = !cmd.Request.Submit;

        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
    }
}
