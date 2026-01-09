using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Validations;
using Tawtheef.Application.Common.Services;
using Tawtheef.Application.Features.Recruitment.Profile.Command.RevisionOperation;
using Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command.SaveOperation;
using Tawtheef.Application.Features.Resources.Commands;
using Tawtheef.Application.Features.Resources.DTOs;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command.RevisionOperation.Save;

public sealed class ReviseProfilePersonalHandler(
    IUnitOfWork uow,
    IMediator mediator,
    UserManager<User> userManager,
    IProfileStepValidationService validationService
    ) : ICommandHandler<ReviseProfilePersonalCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(ReviseProfilePersonalCommand cmd, CancellationToken ct)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(p => p.Id == cmd.UserId, ct);
        if (user is null) return Result.Fail<Unit>(ErrorsCodes.UserNotFound);
        
        var profile = await UserProfileLoader.GetFullProfileByUserId(uow, cmd.UserId, true, ct);
        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);

        if (profile.Status != UserProfileStatus.RequiresUpdate)
            return Result.Fail<Unit>(ErrorsCodes.ProfileLockedUnderReview);
        
        var validationResult = validationService.ValidatePersonal(profile, cmd.Request);
        if (validationResult.IsFailed)
            return Result.Fail<Unit>(validationResult.Errors);

        var r = cmd.Request;
        
        var checkNationalNumber = await uow.GetEntityRepository<UserProfile>()
            .DbSet.AnyAsync(p => p.NationalNumber == r.NationalNumber && p.NationalityId == r.NationalityId
                                 && p.Id != profile.Id, ct);
        if (checkNationalNumber)
            return Result.Fail<Unit>(ErrorsCodes.DuplicateNationalNumber);

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
        }

        await ReviewItemSaveHelper.UpdateSectionStatusAsync(uow, profile, ProfileSection.Personal, ct);
        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
        
        
        async Task<Result<Guid?>> UploadIfNeededAsync(IFormFile? file, Guid? existingId)
        {
            if (file is null || file.Length == 0)
                return Result.Ok(existingId);

            var uploadPath   = await UserProfileUploadPathFactory.CreateAsync(cmd.UserId, "sponsor-card", file, false, ct);
            var uploadResult = await mediator.SendCommandAsync<UploadAttachmentCommand, IResult<UploadAttachmentRequest>>(
                new UploadAttachmentCommand(cmd.UserId, uploadPath.FileId, uploadPath.Path, uploadPath.Hash, file),
                ct);
            if (uploadResult.IsFailed)
                return Result.Fail<Guid?>(uploadResult.Errors);

            return Result.Ok<Guid?>(uploadResult.Value.ResourceId);
        }
    }
}
