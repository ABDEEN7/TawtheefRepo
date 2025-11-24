using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Recruitment.Profile.Command;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command;

public sealed class SaveUserProfileHandler(
    IUnitOfWork uow
) : IRequestHandler<SaveUserProfileCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(SaveUserProfileCommand cmd, CancellationToken ct)
    {
        var repo = uow.GetEntityRepository<UserProfile>();

        var profile = await repo.DbSet
            .Include(p => p.Qualifications)
            .Include(p => p.Experiences)
            .Include(p => p.TrainingCourses)
            .Include(p => p.Skills)
            .Include(p => p.Languages)
            .Include(p => p.AdditionalAttachments)
            .FirstOrDefaultAsync(p => p.UserId == cmd.UserId, ct);

        if (profile is null)
        {
            profile = new UserProfile
            {
                UserId = cmd.UserId,
                IsDraft = true
            };
            await repo.AddAsync(profile);
        }

        var r = cmd.Request;

        // Map scalar fields (only if not null if you want partial updates)
        profile.CandidateTypeId   = r.CandidateTypeId;
        profile.TargetEntityId    = r.TargetEntityId;
        if (r.NationalNumber.HasValue)  profile.NationalNumber = r.NationalNumber.Value;
        if (r.BirthDate.HasValue)       profile.BirthDate      = r.BirthDate.Value;

        if (r.NationalityId.HasValue)   profile.NationalityId  = r.NationalityId.Value;
        if (r.GenderId.HasValue)        profile.GenderId       = r.GenderId.Value;
        if (r.ReligionId.HasValue)      profile.ReligionId     = r.ReligionId.Value;
        if (r.MaritalStatusId.HasValue) profile.MaritalStatusId = r.MaritalStatusId.Value;
        if (r.ChildrenCount.HasValue)   profile.ChildrenCount  = r.ChildrenCount.Value;

        if (r.ResidenceCountryId.HasValue)
            profile.ResidenceCountryId = r.ResidenceCountryId.Value;

        profile.HasDisability   = r.HasDisability;
        profile.DisabilityDetails = r.DisabilityDetails;

        profile.IsDraft = !r.Submit;

        await uow.SaveChangesAsync(ct);

        return Result.Ok(Unit.Value);
    }
}
