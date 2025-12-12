using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Validations;
using Tawtheef.Application.Common.Services;
using Tawtheef.Application.Features.Recruitment.Profile.Command;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command.SaveOperation;

public sealed class SaveProfileContactHandler(
    IUnitOfWork uow,
    IMediator mediator,
    IProfileReviewService reviewService,
    IProfileStepValidationService validationService
) : IRequestHandler<SaveProfileContactCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(SaveProfileContactCommand cmd, CancellationToken ct)
    {
        var profileRepo = uow.GetEntityRepository<UserProfile>();
        var profile = await profileRepo.DbSet
            .Include(p => p.ResidenceAddress)
            .SingleOrDefaultAsync(p => p.UserId == cmd.UserId, ct);

        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);

        if (profile.Status is UserProfileStatus.Submitted or UserProfileStatus.UnderReview)
            return Result.Fail<Unit>(ErrorsCodes.ProfileLockedUnderReview);

        var trackChanges = profile.Status == UserProfileStatus.Approved;

        var oldResidenceCountryId = profile.ResidenceCountryId;
        var oldInterviewLocationId = profile.InterviewLocationId;
        var oldAddress = profile.Address;
        var oldNationalAddress = profile.ResidenceAddress is null ? null : new
        {
            profile.ResidenceAddress.ZoneNo,
            profile.ResidenceAddress.StreetNo,
            profile.ResidenceAddress.BuildingNo,
            profile.ResidenceAddress.UnitNo
        };
        var oldCertificateId = profile.ResidenceAddressCertificateId;

        var oldSnapshot = BuildContactSnapshot(profile);

        var validationResult = validationService.ValidateContact(profile, cmd.Request);
        if (validationResult.IsFailed)
            return Result.Fail<Unit>(validationResult.Errors);

        var r = cmd.Request;

        profile.ResidenceCountryId = r.ResidenceCountryId;
        profile.InterviewLocationId = r.InterviewLocationId;
        profile.Address = r.Address;

        if (r.NationalAddress is not null)
        {
            if (profile.ResidenceAddress is null)
            {
                profile.ResidenceAddress =
                    ResidenceAddress.Create(r.NationalAddress.Building, r.NationalAddress.Street,
                        r.NationalAddress.Zone, r.NationalAddress.Unit);
            }
            else
            {

                profile.ResidenceAddress.ZoneNo = r.NationalAddress.Zone;
                profile.ResidenceAddress.StreetNo = r.NationalAddress.Street;
                profile.ResidenceAddress.BuildingNo = r.NationalAddress.Building;
                profile.ResidenceAddress.UnitNo = r.NationalAddress.Unit;
            }

            var idResult = await UploadIfNeededAsync(r.NationalAddress.NationalAddress, profile.ResidenceAddressCertificateId);
            if (idResult.IsFailed)
                return Result.Fail<Unit>(idResult.Errors);
            profile.ResidenceAddressCertificateId = idResult.Value;

        }
        var newSnapshot = BuildContactSnapshot(profile);

        if (trackChanges)
        {
            await reviewService.TouchFieldAsync(profile.Id, ProfileSection.Contact, nameof(UserProfile.ResidenceCountryId), ct, oldResidenceCountryId, profile.ResidenceCountryId);
            await reviewService.TouchFieldAsync(profile.Id, ProfileSection.Contact, nameof(UserProfile.InterviewLocationId), ct, oldInterviewLocationId, profile.InterviewLocationId);
            await reviewService.TouchFieldAsync(profile.Id, ProfileSection.Contact, nameof(UserProfile.Address), ct, oldAddress, profile.Address);
            await reviewService.TouchFieldAsync(profile.Id, ProfileSection.Contact, "ResidenceAddress", ct, oldNationalAddress, profile.ResidenceAddress is null ? null : new
            {
                profile.ResidenceAddress.ZoneNo,
                profile.ResidenceAddress.StreetNo,
                profile.ResidenceAddress.BuildingNo,
                profile.ResidenceAddress.UnitNo
            });
            if (oldCertificateId.HasValue || profile.ResidenceAddressCertificateId.HasValue)
            {
                await reviewService.TouchAttachmentAsync(
                    profile.Id,
                    ProfileSection.Contact,
                    "National Address Certificate",
                    profile.ResidenceAddressCertificateId ?? oldCertificateId!.Value,
                    ct,
                    oldCertificateId,
                    profile.ResidenceAddressCertificateId);
            }
            await reviewService.TouchSectionAsync(profile.Id, ProfileSection.Contact, ct, oldSnapshot, newSnapshot);
        }
        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
        
        async Task<Result<Guid?>> UploadIfNeededAsync(IFormFile? file, Guid? existingId)
        {
            if (file is null || file.Length == 0)
                return Result.Ok(existingId);

            var uploadPath   = await UserProfileUploadPathFactory.CreateAsync(cmd.UserId, "national-address", file, false, ct);
            var uploadResult = await mediator.Send(
                new UploadAttachmentCommand(cmd.UserId, uploadPath.FileId, uploadPath.Path, uploadPath.Hash, file),
                ct);
            if (uploadResult.IsFailed)
                return Result.Fail<Guid?>(uploadResult.Errors);

            return Result.Ok<Guid?>(uploadResult.Value.ResourceId);
        }

        static object BuildContactSnapshot(UserProfile profileEntity) => new
        {
            profileEntity.ResidenceCountryId,
            profileEntity.InterviewLocationId,
            profileEntity.Address,
            profileEntity.ResidenceAddress?.ZoneNo,
            profileEntity.ResidenceAddress?.StreetNo,
            profileEntity.ResidenceAddress?.BuildingNo,
            profileEntity.ResidenceAddress?.UnitNo,
            profileEntity.ResidenceAddressCertificateId
        };
    }
}
