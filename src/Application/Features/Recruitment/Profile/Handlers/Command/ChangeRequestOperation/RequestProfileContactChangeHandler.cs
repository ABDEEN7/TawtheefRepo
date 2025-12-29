using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Validations;
using Tawtheef.Application.Common.Services;
using Tawtheef.Application.Features.Recruitment.Profile.Command;
using Tawtheef.Application.Features.Recruitment.Profile.Command.ChangeRequestOperation;
using Tawtheef.Application.Features.Recruitment.Profile.DTOs;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command.ChangeRequestOperation;

public sealed class RequestProfileContactChangeHandler(
    IUnitOfWork uow,
    IMediator mediator,
    IProfileStepValidationService validationService,
    IProfileReviewService reviewService
) : IRequestHandler<RequestProfileContactChangeCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(RequestProfileContactChangeCommand cmd, CancellationToken ct)
    {
        var profile = await UserProfileLoader.GetFullProfileByUserId(uow, cmd.UserId, ct: ct);
        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);

        if (profile.Status == UserProfileStatus.InCreation)
            return Result.Fail<Unit>(ErrorsCodes.NotSubmitted);

        var validationResult = validationService.ValidateContact(profile, cmd.Request);
        if (validationResult.IsFailed)
            return Result.Fail<Unit>(validationResult.Errors);

        var currentSnapshot = ContactSectionSnapshot.From(profile);

        var naUpload = await UploadIfNeededAsync(
            cmd.Request.NationalAddress?.NationalAddress,
            profile.ResidenceAddress?.CertificateId);
        if (naUpload.IsFailed)
            return Result.Fail<Unit>(naUpload.Errors);

        var nextSnapshot = currentSnapshot.ApplyRequest(cmd.Request, naUpload.Value);
        if (nextSnapshot != currentSnapshot)
        {
            await reviewService.TouchSectionAsync(profile.Id, ProfileSection.Contact, cmd.UserId, ct, currentSnapshot, nextSnapshot);
            await uow.SaveChangesAsync(ct);
        }

        return Result.Ok(Unit.Value);

        async Task<Result<Guid?>> UploadIfNeededAsync(IFormFile? file, Guid? existingId)
        {
            if (file is null || file.Length == 0)
                return Result.Ok(existingId);

            var uploadPath = await UserProfileUploadPathFactory.CreateAsync(cmd.UserId, "national-address", file, false, ct);
            var uploadResult = await mediator.Send(
                new UploadAttachmentCommand(cmd.UserId, uploadPath.FileId, uploadPath.Path, uploadPath.Hash, file),
                ct);
            if (uploadResult.IsFailed)
                return Result.Fail<Guid?>(uploadResult.Errors);

            return Result.Ok<Guid?>(uploadResult.Value.ResourceId);
        }
    }
}

file sealed record ContactSectionSnapshot
{
    public Guid? ResidenceCountryId { get; init; }
    public Guid? InterviewLocationId { get; init; }
    public string? Address { get; init; }
    public Guid? OfficeId { get; init; }
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
            Address = request.Address ?? Address,
            OfficeId = request.OfficeId ?? OfficeId,
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

