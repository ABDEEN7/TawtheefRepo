using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Recruitment.Profile.Command;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command;

public sealed class SaveProfilePersonalHandler(
    IUnitOfWork uow,
    UserManager<User> userManager
    ) : IRequestHandler<SaveProfilePersonalCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(SaveProfilePersonalCommand cmd, CancellationToken ct)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(p => p.Id == cmd.UserId, ct);
        if (user is null) return Result.Fail<Unit>(ErrorsCodes.UserNotFound);
        
        var profileRepo = uow.GetEntityRepository<UserProfile>();
        var profile = await profileRepo.DbSet
            .FirstOrDefaultAsync(p => p.UserId == cmd.UserId, ct);

        if (profile is null)
        {
            profile = new UserProfile
            {
                UserId  = cmd.UserId,
                IsDraft = true
            };
            await profileRepo.AddAsync(profile);
        }

        var r = cmd.Request;

        user.FullNameAr  = r.FullNameAr ?? user.FullNameAr;
        user.FullNameEn = r.FullNameEn ?? user.FullNameEn;
        profile.NationalNumber = r.NationalNumber ?? profile.NationalNumber;
        profile.BirthDate      = r.BirthDate ?? profile.BirthDate;

        profile.NationalityId   = r.NationalityId ?? profile.NationalityId;
        profile.GenderId        = r.GenderId;
        profile.ReligionId      = r.ReligionId;
        profile.MaritalStatusId = r.MaritalStatusId ?? profile.MaritalStatusId;
        profile.ChildrenCount   = r.ChildrenCount ?? profile.ChildrenCount;

        profile.HasDisability    = r.HasDisability;
        profile.DisabilityDetails = r.DisabilityDetails;


        if (!string.IsNullOrWhiteSpace(r.SponsorEmployerName) &&
            !string.IsNullOrWhiteSpace(r.SponsorEmployerNumber) &&
            !string.IsNullOrWhiteSpace(r.SponsorCardFileName) &&
            !string.IsNullOrWhiteSpace(r.SponsorCardResourceId?.ToString()))
        {
            profile.SponsorProfile = new SponsorProfile
            {
                SponsorTypeId = r.SponsorTypeId!.Value,
                SponsorName = r.SponsorEmployerName,
                SponsorNumber = r.SponsorEmployerNumber,
                SponsorCardId = r.SponsorCardResourceId
            };
        }

        profile.IsDraft = true;

        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
    }
}
