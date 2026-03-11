using Application.Recruitment.Features.Profile.Command.RevisionOperation;
using Application.Recruitment.Features.Profile.Handlers.Command.SaveOperation;
using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Validations;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Recruitment.Features.Profile.Handlers.Command.RevisionOperation.Save;

public sealed class ReviseProfilePrereqAttachmentsHandler(
    IUnitOfWork uow,
    IProfileStepValidationService validationService,
    IMediator mediator
) : IRequestHandler<ReviseProfilePrereqAttachmentsCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(ReviseProfilePrereqAttachmentsCommand cmd, CancellationToken ct)
    {
        var profile = await UserProfileLoader.GetFullProfileByUserId(uow, cmd.UserId, true, ct);
        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);


        if (profile.Status is not UserProfileStatus.RequiresUpdate && profile.Status is not UserProfileStatus.Submitted)
            return Result.Fail<Unit>(ErrorsCodes.ProfileLockedUnderReview);

        var vr = validationService.ValidateAttachments(profile);
        if (vr.IsFailed)
            return Result.Fail<Unit>(vr.Errors);

        if (cmd.Request.Birthday is not null)
        {
            var saver = new ProfileBasicAttachmentSaver(uow, mediator);
            var newId = await saver.SaveOrReplaceAsync(
                profile,
                section: ProfileSection.Prerequisites,
                meta: cmd.Request.Birthday,
                file: cmd.Request.BirthdayCertificate,
                currentProfileResourceId: profile.BirthdayCertificateId,
                folder: ProfileFileCategories.BirthCertificate,
                ct);

            if (newId.IsFailed) return Result.Fail<Unit>(newId.Errors);
            profile.BirthdayCertificateId = newId.Value;
            if (profile.Status == UserProfileStatus.RequiresUpdate || profile.Status == UserProfileStatus.Submitted)
                await ReviewItemSaveHelper.UpdateSectionStatusAsync(uow, profile, ProfileSection.Prerequisites, ct);
        }

        if (cmd.Request.Marriage is not null)
        {
            var saver = new ProfileBasicAttachmentSaver(uow, mediator);
            var newId = await saver.SaveOrReplaceAsync(
                profile,
                section: ProfileSection.Prerequisites,
                meta: cmd.Request.Marriage,
                file: cmd.Request.MarriageCertificate,
                currentProfileResourceId: profile.MarriageCertificateId,
                folder: ProfileFileCategories.MarriageCertificate,
                ct);

            if (newId.IsFailed) return Result.Fail<Unit>(newId.Errors);
            profile.MarriageCertificateId = newId.Value;
            if (profile.Status == UserProfileStatus.RequiresUpdate || profile.Status == UserProfileStatus.Submitted)
                await ReviewItemSaveHelper.UpdateSectionStatusAsync(uow, profile, ProfileSection.Prerequisites, ct);
        }

        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
    }
}

