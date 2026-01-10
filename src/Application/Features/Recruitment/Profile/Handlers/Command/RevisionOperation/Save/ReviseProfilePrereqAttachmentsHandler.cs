using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Validations;
using Tawtheef.Application.Features.Recruitment.Profile.Command.RevisionOperation;
using Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command.SaveOperation;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command.RevisionOperation.Save;

public sealed class ReviseProfilePrereqAttachmentsHandler(
    IUnitOfWork uow,
    IProfileStepValidationService validationService,
    IMediator mediator
) : ICommandHandler<ReviseProfilePrereqAttachmentsCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(ReviseProfilePrereqAttachmentsCommand cmd, CancellationToken ct)
    {
        var profile = await UserProfileLoader.GetFullProfileByUserId(uow, cmd.UserId, true, ct);
        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);


        if (profile.Status is not UserProfileStatus.RequiresUpdate)
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
                folder: "birth-certificate",
                ct);

            if (newId.IsFailed) return Result.Fail<Unit>(newId.Errors);
            profile.BirthdayCertificateId = newId.Value;
            if (profile.Status == UserProfileStatus.RequiresUpdate)
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
                folder: "marriage-certificate",
                ct);

            if (newId.IsFailed) return Result.Fail<Unit>(newId.Errors);
            profile.MarriageCertificateId = newId.Value;
            if (profile.Status == UserProfileStatus.RequiresUpdate)
                await ReviewItemSaveHelper.UpdateSectionStatusAsync(uow, profile, ProfileSection.Prerequisites, ct);
        }

        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
    }
}
