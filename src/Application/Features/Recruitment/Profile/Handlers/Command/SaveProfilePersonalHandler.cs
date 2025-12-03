using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Services;
using Tawtheef.Application.Features.Recruitment.Profile.Command;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command;

public sealed class SaveProfilePersonalHandler(
    IUnitOfWork uow, IMediator mediator,UserManager<User> userManager, IProfileReviewService reviewService
    ) : IRequestHandler<SaveProfilePersonalCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(SaveProfilePersonalCommand cmd, CancellationToken ct)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(p => p.Id == cmd.UserId, ct);
        if (user is null) return Result.Fail<Unit>(ErrorsCodes.UserNotFound);
        
        var profileRepo = uow.GetEntityRepository<UserProfile>();
        var profile = await profileRepo.DbSet
            .Include(p => p.SponsorProfile)
            .SingleOrDefaultAsync(p => p.UserId == cmd.UserId, ct);

        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);

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
        profile.DisabilityDetails = r.HasDisability
            ? r.DisabilityDetails
            : null;


        if (!string.IsNullOrWhiteSpace(r.SponsorEmployerName) && !string.IsNullOrWhiteSpace(r.SponsorEmployerNumber))
        {
            var idResult = await UploadIfNeededAsync(r.SponsorCard, profile.SponsorProfile?.SponsorCardId);
            if (idResult.IsFailed)
                return Result.Fail<Unit>(idResult.Errors);
            
            if (profile.SponsorProfile is null)
            {
                profile.SponsorProfile = new SponsorProfile
                {
                    SponsorName = r.SponsorEmployerName,
                    SponsorNumber = r.SponsorEmployerNumber,
                    SponsorCardId = idResult.Value
                };
            }
            else
            {
                profile.SponsorProfile.SponsorName = r.SponsorEmployerName;
                profile.SponsorProfile.SponsorNumber = r.SponsorEmployerNumber;
                profile.SponsorProfile.SponsorCardId = idResult.Value;
            }

            if (idResult.Value.HasValue)
            {
                await reviewService.TouchAttachmentAsync(
                    profile.Id,
                    Domain.Entities.Recruitment.ProfileSection.Personal,
                    "Sponsor Card",
                    idResult.Value.Value,
                    ct);
            }
        }

        profile.IsDraft = true;

        await reviewService.TouchSectionAsync(profile.Id, Domain.Entities.Recruitment.ProfileSection.Personal, ct);
        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
        
        
        async Task<Result<Guid?>> UploadIfNeededAsync(IFormFile? file, Guid? existingId)
        {
            if (file is null || file.Length == 0)
                return Result.Ok(existingId);

            var uploadPath   = await UserProfileUploadPathFactory.CreateAsync(cmd.UserId, "sponsor-card", file, false, ct);
            var uploadResult = await mediator.Send(
                new UploadAttachmentCommand(cmd.UserId, uploadPath.FileId, uploadPath.Path, uploadPath.Hash, file),
                ct);
            if (uploadResult.IsFailed)
                return Result.Fail<Guid?>(uploadResult.Errors);

            return Result.Ok<Guid?>(uploadResult.Value.ResourceId);
        }
    }
}
