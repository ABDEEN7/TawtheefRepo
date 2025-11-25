using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Recruitment.Profile.Command;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command;

public sealed class SaveProfileEducationHandler(
    IUnitOfWork uow
) : IRequestHandler<SaveProfileEducationCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(SaveProfileEducationCommand cmd, CancellationToken ct)
    {
        var repo = uow.GetEntityRepository<UserProfile>();

        var profile = await repo.DbSet
            .Include(p => p.Qualifications)
            .FirstOrDefaultAsync(p => p.UserId == cmd.UserId, ct);

        if (profile is null)
        {
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);
        }

        var req = cmd.Request;

        profile.Qualifications ??= [];
        
        var incomingIds = req.Degrees
            .Where(d => d.Id.HasValue)
            .Select(d => d.Id!.Value)
            .ToHashSet();

        var toRemove = profile.Qualifications
            .Where(q => !incomingIds.Contains(q.Id))
            .ToList();
        toRemove.ForEach(q => profile.Qualifications.Remove(q));

        // 3) Upsert لكل Degree
        foreach (var dto in req.Degrees)
        {
            Qualification entity;

            if (dto.Id is Guid existingId)
            {
                entity = profile.Qualifications
                             .FirstOrDefault(x => x.Id == existingId)
                         ?? new Qualification { UserProfileId = profile.Id };
            }
            else
            {
                entity = new Qualification{ UserProfileId = profile.Id };
                profile.Qualifications.Add(entity);
            }

            entity.LevelId        = dto.LevelId;
            entity.MajorId        = dto.MajorId;
            entity.UniversityId   = dto.UniversityId;
            entity.GraduationYear = dto.GraduationYear;
            entity.StudyTypeId    = dto.StudyTypeId;
            entity.GPA            = dto.GPA;
            entity.RatingId       = dto.RatingId;
            entity.CountryId      = dto.CountryId;

            if (dto.CertificateResourceId.HasValue)
                entity.CertificateId = dto.CertificateResourceId.Value;
        }

        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
    }
}
