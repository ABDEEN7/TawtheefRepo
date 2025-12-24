using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Validations;
using Tawtheef.Application.Common.Services;
using Tawtheef.Application.Features.Recruitment.Profile.Command;
using Tawtheef.Application.Features.Recruitment.Profile.Command.SaveOperation;
using Tawtheef.Application.Features.Recruitment.Profile.DTOs;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command.SaveOperation;

public sealed class SaveProfileContactHandler(
    IUnitOfWork uow,
    IMediator mediator,
    IProfileStepValidationService validationService
) : IRequestHandler<SaveProfileContactCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(SaveProfileContactCommand cmd, CancellationToken ct)
    {
        var profile = await UserProfileLoader.GetFullProfile(uow, cmd.UserId, true, ct);
        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);

        var validationResult = validationService.ValidateContact(profile, cmd.Request);
        if (validationResult.IsFailed)
            return Result.Fail<Unit>(validationResult.Errors);

        var r = cmd.Request;

        if (profile.Status is not UserProfileStatus.InCreation)
            return Result.Fail<Unit>(ErrorsCodes.ProfileLockedUnderReview);

        profile.ResidenceCountryId = r.ResidenceCountryId;
        profile.InterviewLocationId = r.InterviewLocationId;
        profile.Address = r.Address;

        if (r.NationalAddress is not null)
        {
            var idResult = await UploadIfNeededAsync(r.NationalAddress.NationalAddress, profile.ResidenceAddress?.CertificateId);
            if (idResult.IsFailed)
                return Result.Fail<Unit>(idResult.Errors);
            
            if (profile.ResidenceAddress is null)
            {
                profile.ResidenceAddress =
                    ResidenceAddress.Create(r.NationalAddress.Building, r.NationalAddress.Street,
                        r.NationalAddress.Zone, r.NationalAddress.Unit, idResult.Value!.Value);
            }
            else
            {

                profile.ResidenceAddress.ZoneNo = r.NationalAddress.Zone;
                profile.ResidenceAddress.StreetNo = r.NationalAddress.Street;
                profile.ResidenceAddress.BuildingNo = r.NationalAddress.Building;
                profile.ResidenceAddress.UnitNo = r.NationalAddress.Unit;
                profile.ResidenceAddress.CertificateId = idResult.Value!.Value;
            }

        }
        var newSnapshot = BuildContactSnapshot(profile);

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
            ResidenceAddressCertificateId = profileEntity.ResidenceAddress?.CertificateId
        };
    }
}

file sealed record ContactSectionSnapshot
{
    public Guid? ResidenceCountryId { get; init; }
    public Guid? InterviewLocationId { get; init; }
    public string? Address { get; init; }
    public int? Zone { get; init; }
    public int? Street { get; init; }
    public int? Building { get; init; }
    public int? Unit { get; init; }
    public Guid? NationalAddressCertificateId { get; init; }

    public static ContactSectionSnapshot From(UserProfile profile) => new()
    {
        ResidenceCountryId = profile.ResidenceCountryId,
        InterviewLocationId = profile.InterviewLocationId,
        Address = profile.Address,
        Zone = profile.ResidenceAddress?.ZoneNo,
        Street = profile.ResidenceAddress?.StreetNo,
        Building = profile.ResidenceAddress?.BuildingNo,
        Unit = profile.ResidenceAddress?.UnitNo,
        NationalAddressCertificateId = profile.ResidenceAddress?.CertificateId
    };

    public ContactSectionSnapshot ApplyRequest(SaveProfileContactRequest request, Guid? certificateResourceId)
    {
        var snapshot = this with
        {
            ResidenceCountryId = request.ResidenceCountryId,
            InterviewLocationId = request.InterviewLocationId,
            Address = request.Address ?? Address
        };

        if (request.NationalAddress is not null)
        {
            snapshot = snapshot with
            {
                Zone = request.NationalAddress.Zone,
                Street = request.NationalAddress.Street,
                Building = request.NationalAddress.Building,
                Unit = request.NationalAddress.Unit,
                NationalAddressCertificateId = certificateResourceId ?? NationalAddressCertificateId
            };
        }

        return snapshot;
    }
}
