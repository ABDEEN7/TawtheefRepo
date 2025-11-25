using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Recruitment.Profile.Command;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command;

public sealed class SaveProfileContactHandler(
    IUnitOfWork uow
) : IRequestHandler<SaveProfileContactCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(SaveProfileContactCommand cmd, CancellationToken ct)
    {
        var profileRepo = uow.GetEntityRepository<UserProfile>();
        var profile = await profileRepo.DbSet
            .Include(p => p.ResidenceAddress)
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

        var r = cmd.Request;

        profile.ResidenceCountryId = r.ResidenceCountryId;
        profile.Address            = r.Address;
        profile.InterviewLocationId = r.InterviewLocationId;

        // National Address
        if (r.NationalAddress is not null)
        {
            if (profile.ResidenceAddress is null)
                profile.ResidenceAddress = new ResidenceAddress()
                {
                    Address = r.Address ?? string.Empty,
                    ZoneNo = r.NationalAddress.Zone,
                    StreetNo = r.NationalAddress.Street,
                    BuildingNo = r.NationalAddress.Building,
                    UnitNo = r.NationalAddress.Unit,
                };

            profile.ResidenceAddress.Address   = r.Address ?? string.Empty;
            profile.ResidenceAddress.ZoneNo    = r.NationalAddress.Zone;
            profile.ResidenceAddress.StreetNo  = r.NationalAddress.Street;
            profile.ResidenceAddress.BuildingNo = r.NationalAddress.Building;
            profile.ResidenceAddress.UnitNo    = r.NationalAddress.Unit;
            
            profile.NationalCardId = r.NationalAddress.ResourceId;
        }

        profile.IsDraft = true;

        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
    }
}
