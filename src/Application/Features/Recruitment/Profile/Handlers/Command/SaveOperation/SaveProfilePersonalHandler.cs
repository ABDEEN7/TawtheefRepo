using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Validations;
using Tawtheef.Application.Common.Services;
using Tawtheef.Application.Features.Recruitment.Profile.Command;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command.SaveOperation;

public sealed class SaveProfilePersonalHandler(
    IUnitOfWork uow,
    IMediator mediator,
    UserManager<User> userManager,
    IProfileReviewService reviewService,
    IProfileStepValidationService validationService
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

        if (profile.Status is UserProfileStatus.Submitted or UserProfileStatus.UnderReview)
            return Result.Fail<Unit>(ErrorsCodes.ProfileLockedUnderReview);

        var trackChanges = profile.Status == UserProfileStatus.Approved;

        var validationResult = validationService.ValidatePersonal(profile, cmd.Request);
        if (validationResult.IsFailed)
            return Result.Fail<Unit>(validationResult.Errors);

        var r = cmd.Request;

        var oldFullNameAr = user.FullNameAr;
        var oldFullNameEn = user.FullNameEn;
        var oldNationalNumber = profile.NationalNumber;
        var oldBirthDate = profile.BirthDate;
        var oldQidExpiry = profile.QIDExpiry;
        var oldNationalityId = profile.NationalityId;
        var oldGenderId = profile.GenderId;
        var oldReligionId = profile.ReligionId;
        var oldMaritalStatusId = profile.MaritalStatusId;
        var oldChildrenCount = profile.ChildrenCount;
        var oldHasDisability = profile.HasDisability;
        var oldDisabilityDetails = profile.DisabilityDetails;

        var oldSnapshot = BuildPersonalSnapshot(user, profile);

        user.FullNameAr  = r.FullNameAr ?? user.FullNameAr;
        user.FullNameEn = r.FullNameEn ?? user.FullNameEn;
        profile.NationalNumber = r.NationalNumber ?? profile.NationalNumber;
        profile.BirthDate      = r.BirthDate ?? profile.BirthDate;
        profile.QIDExpiry      = r.QIDExpiry ?? profile.QIDExpiry;

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
                    SponsorTypeId = r.SponsorTypeId!.Value,
                    SponsorName = r.SponsorEmployerName,
                    SponsorNumber = r.SponsorEmployerNumber,
                    QIDExpiry = r.QIDExpiry!.Value,
                    SponsorCardId = idResult.Value,
                };
            }
            else
            {
                profile.SponsorProfile.SponsorTypeId = r.SponsorTypeId!.Value;
                profile.SponsorProfile.SponsorName = r.SponsorEmployerName;
                profile.SponsorProfile.SponsorNumber = r.SponsorEmployerNumber;
                profile.SponsorProfile.QIDExpiry = r.QIDExpiry!.Value;
                profile.SponsorProfile.SponsorCardId = idResult.Value;
            }

            if (trackChanges && idResult.Value.HasValue)
            {
                await reviewService.TouchAttachmentAsync(
                    profile.Id,
                    ProfileSection.Personal,
                    "Sponsor Card",
                    idResult.Value.Value,
                    ct);
            }
        }

        var newSnapshot = BuildPersonalSnapshot(user, profile);

        if (trackChanges)
        {
            await reviewService.TouchFieldAsync(profile.Id, ProfileSection.Personal, nameof(User.FullNameAr), ct, oldFullNameAr, user.FullNameAr);
            await reviewService.TouchFieldAsync(profile.Id, ProfileSection.Personal, nameof(User.FullNameEn), ct, oldFullNameEn, user.FullNameEn);
            await reviewService.TouchFieldAsync(profile.Id, ProfileSection.Personal, nameof(UserProfile.NationalNumber), ct, oldNationalNumber, profile.NationalNumber);
            await reviewService.TouchFieldAsync(profile.Id, ProfileSection.Personal, nameof(UserProfile.BirthDate), ct, oldBirthDate, profile.BirthDate);
            await reviewService.TouchFieldAsync(profile.Id, ProfileSection.Personal, nameof(UserProfile.QIDExpiry), ct, oldQidExpiry, profile.QIDExpiry);
            await reviewService.TouchFieldAsync(profile.Id, ProfileSection.Personal, nameof(UserProfile.NationalityId), ct, oldNationalityId, profile.NationalityId);
            await reviewService.TouchFieldAsync(profile.Id, ProfileSection.Personal, nameof(UserProfile.GenderId), ct, oldGenderId, profile.GenderId);
            await reviewService.TouchFieldAsync(profile.Id, ProfileSection.Personal, nameof(UserProfile.ReligionId), ct, oldReligionId, profile.ReligionId);
            await reviewService.TouchFieldAsync(profile.Id, ProfileSection.Personal, nameof(UserProfile.MaritalStatusId), ct, oldMaritalStatusId, profile.MaritalStatusId);
            await reviewService.TouchFieldAsync(profile.Id, ProfileSection.Personal, nameof(UserProfile.ChildrenCount), ct, oldChildrenCount, profile.ChildrenCount);
            await reviewService.TouchFieldAsync(profile.Id, ProfileSection.Personal, nameof(UserProfile.HasDisability), ct, oldHasDisability, profile.HasDisability);
            await reviewService.TouchFieldAsync(profile.Id, ProfileSection.Personal, nameof(UserProfile.DisabilityDetails), ct, oldDisabilityDetails, profile.DisabilityDetails);
            await reviewService.TouchSectionAsync(profile.Id, ProfileSection.Personal, ct, oldSnapshot, newSnapshot);
        }
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

        static object BuildPersonalSnapshot(User userEntity, UserProfile profileEntity) => new
        {
            userEntity.FullNameAr,
            userEntity.FullNameEn,
            profileEntity.NationalNumber,
            profileEntity.BirthDate,
            profileEntity.QIDExpiry,
            profileEntity.NationalityId,
            profileEntity.GenderId,
            profileEntity.ReligionId,
            profileEntity.MaritalStatusId,
            profileEntity.ChildrenCount,
            profileEntity.HasDisability,
            profileEntity.DisabilityDetails
        };
    }
}
