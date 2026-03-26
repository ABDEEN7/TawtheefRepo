using Application.Recruitment.Features.Profile.Command.ChangeRequestOperation;
using Application.Recruitment.Features.Profile.DTOs.SaveOperation;
using Application.Recruitment.Features.Profile.Policies;
using MediatR;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Services;
using Tawtheef.Application.Common.Validations;
using Tawtheef.Application.Features.Resources.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Recruitment.Features.Profile.Handlers.Command.ChangeRequestOperation;

public sealed class RequestProfilePersonalChangeHandler(
    IUnitOfWork uow,
    IMediator mediator,
    UserManager<User> userManager,
    IProfileStepValidationService validationService,
    IProfileReviewService reviewService
) : IRequestHandler<RequestProfilePersonalChangeCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(RequestProfilePersonalChangeCommand cmd, CancellationToken ct)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(p => p.Id == cmd.UserId, ct);
        if (user is null) return Result.Fail<Unit>(ErrorsCodes.UserNotFound);

        var profile = await UserProfileLoader.GetFullProfileByUserId(uow, cmd.UserId, ct: ct);
        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);

        if (profile.Status == UserProfileStatus.InCreation)
            return Result.Fail<Unit>(ErrorsCodes.NotSubmitted);

        var validationResult = validationService.ValidatePersonal(profile, new(cmd.Request.SponsorEmployerName, cmd.Request.SponsorEmployerNumber, cmd.Request.SponsorCardFileName, cmd.Request.SponsorCard));
        if (validationResult.IsFailed)
            return Result.Fail<Unit>(validationResult.Errors);

        var identityValidation = VerifiedIdentityPolicy.EnsureIdentityHydrated(user, profile);
        if (identityValidation.IsFailed)
            return Result.Fail<Unit>(identityValidation.Errors);

        var isLockedProvider = VerifiedIdentityProviders.IsLockedProvider(profile.Provider);
        var currentSnapshot = PersonalSectionSnapshot.From(user, profile);

        var sponsorCardUpload = await UploadIfNeededAsync(cmd.Request.SponsorCard, null);
        if (sponsorCardUpload.IsFailed)
            return Result.Fail<Unit>(sponsorCardUpload.Errors);

        var nextSnapshot = currentSnapshot.ApplyRequest(cmd.Request, sponsorCardUpload.Value, isLockedProvider);
        if (nextSnapshot == currentSnapshot)
            return Result.Ok(Unit.Value);

        await reviewService.TouchSectionAsync(profile.Id, ProfileSection.Personal, cmd.UserId, ct, currentSnapshot, nextSnapshot);
        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);

        async Task<Result<Guid?>> UploadIfNeededAsync(IFormFile? file, Guid? existingId)
        {
            if (file is null || file.Length == 0)
                return Result.Ok(existingId);

            var uploadPath = await UserProfileUploadPathFactory.CreateAsync(cmd.UserId, ProfileFileCategories.SponsorCard, file, false, ct);
            var uploadResult = await mediator.Send(
                new UploadAttachmentCommand(cmd.UserId, uploadPath.FileId, uploadPath.Path, uploadPath.Hash, file),
                ct);
            if (uploadResult.IsFailed)
                return Result.Fail<Guid?>(uploadResult.Errors);

            return Result.Ok<Guid?>(uploadResult.Value.ResourceId);
        }
    }
}

file sealed record PersonalSectionSnapshot
{
    public string? FullNameAr { get; init; }
    public string? FullNameEn { get; init; }
    public string? NationalNumber { get; init; }
    public DateOnly? QidExpiry { get; init; }
    public DateOnly? BirthDate { get; init; }
    public Guid? NationalityId { get; init; }
    public Guid? GenderId { get; init; }
    public Guid? ReligionId { get; init; }
    public Guid? MaritalStatusId { get; init; }
    public int? ChildrenCount { get; init; }
    public bool HasDisability { get; init; }
    public string? DisabilityDetails { get; init; }
    public Guid? SponsorTypeId { get; init; }
    public string? SponsorEmployerName { get; init; }
    public string? SponsorEmployerNumber { get; init; }
    public DateOnly? SponsorQidExpiry { get; init; }
    public Guid? SponsorCardResourceId { get; init; }

    public static PersonalSectionSnapshot From(User user, UserProfile profile)
    {
        return new PersonalSectionSnapshot
        {
            FullNameAr = user.FullNameAr,
            FullNameEn = user.FullNameEn,
            NationalNumber = profile.NationalNumber,
            QidExpiry = profile.QIDExpiry,
            BirthDate = profile.BirthDate,
            NationalityId = profile.NationalityId,
            GenderId = profile.GenderId,
            ReligionId = profile.ReligionId,
            MaritalStatusId = profile.MaritalStatusId,
            ChildrenCount = profile.ChildrenCount,
            HasDisability = profile.HasDisability,
            DisabilityDetails = profile.DisabilityDetails,
            SponsorTypeId = profile.SponsorProfile?.SponsorTypeId,
            SponsorEmployerName = profile.SponsorProfile?.SponsorName,
            SponsorEmployerNumber = profile.SponsorProfile?.SponsorNumber,
            SponsorQidExpiry = profile.SponsorProfile?.QIDExpiry,
            SponsorCardResourceId = profile.SponsorProfile?.SponsorCardId
        };
    }

    public PersonalSectionSnapshot ApplyRequest(SaveProfilePersonalRequest request, Guid? sponsorCardResourceId, bool isLockedProvider)
    {
        var snapshot = this with
        {
            FullNameAr = isLockedProvider ? (string.IsNullOrWhiteSpace(FullNameAr) ? request.FullNameAr ?? FullNameAr : FullNameAr) : request.FullNameAr ?? FullNameAr,
            FullNameEn = isLockedProvider ? (string.IsNullOrWhiteSpace(FullNameEn) ? request.FullNameEn ?? FullNameEn : FullNameEn) : request.FullNameEn ?? FullNameEn,
            NationalNumber = isLockedProvider ? (string.IsNullOrWhiteSpace(NationalNumber) ? request.NationalNumber ?? NationalNumber : NationalNumber) : request.NationalNumber ?? NationalNumber,
            QidExpiry = isLockedProvider ? QidExpiry ?? request.QIDExpiry : request.QIDExpiry ?? QidExpiry,
            BirthDate = isLockedProvider ? BirthDate ?? request.BirthDate : request.BirthDate ?? BirthDate,
            NationalityId = isLockedProvider ? NationalityId ?? request.NationalityId : request.NationalityId ?? NationalityId,
            GenderId = isLockedProvider ? GenderId ?? request.GenderId : request.GenderId ?? GenderId,
            ReligionId = request.ReligionId ?? ReligionId,
            MaritalStatusId = request.MaritalStatusId ?? MaritalStatusId,
            ChildrenCount = request.ChildrenCount ?? ChildrenCount,
            HasDisability = request.HasDisability,
            DisabilityDetails = request.HasDisability ? request.DisabilityDetails : null
        };

        if (!string.IsNullOrWhiteSpace(request.SponsorEmployerName) &&
            !string.IsNullOrWhiteSpace(request.SponsorEmployerNumber) &&
            request.SponsorTypeId.HasValue)
        {
            snapshot = snapshot with
            {
                SponsorTypeId = request.SponsorTypeId,
                SponsorEmployerName = request.SponsorEmployerName,
                SponsorEmployerNumber = request.SponsorEmployerNumber,
                SponsorQidExpiry = request.SponsorQidExpiry ?? SponsorQidExpiry,
                SponsorCardResourceId = sponsorCardResourceId ?? SponsorCardResourceId
            };
        }

        return snapshot;
    }
}


