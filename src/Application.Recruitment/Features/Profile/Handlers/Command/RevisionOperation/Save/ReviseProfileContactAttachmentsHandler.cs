using Application.Recruitment.Features.Profile.Command.RevisionOperation;
using Application.Recruitment.Features.Profile.Handlers.Command.SaveOperation;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Validations;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Recruitment.Features.Profile.Handlers.Command.RevisionOperation.Save;

public sealed class ReviseProfileContactAttachmentsHandler(
    IUnitOfWork uow,
    IProfileStepValidationService validationService,
    IMediator mediator
) : ICommandHandler<ReviseProfileContactAttachmentsCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(ReviseProfileContactAttachmentsCommand cmd, CancellationToken ct)
    {
        var profile = await UserProfileLoader.GetFullProfileByUserId(uow, cmd.UserId, true, ct);
        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);


        if (profile.Status is not UserProfileStatus.RequiresUpdate)
            return Result.Fail<Unit>(ErrorsCodes.ProfileLockedUnderReview);

        var vr = validationService.ValidateAttachments(profile);
        if (vr.IsFailed)
            return Result.Fail<Unit>(vr.Errors);

        if (profile.ResidenceAddress is null)
            return Result.Fail<Unit>(ErrorsCodes.ResidenceAddressNotFound);

        if (cmd.Request.ResidenceAddress is not null)
        {
            var current = profile.ResidenceAddress.CertificateId;

            var saver = new ProfileBasicAttachmentSaver(uow, mediator);
            var newId = await saver.SaveOrReplaceAsync(
                profile,
                section: ProfileSection.Contact,
                meta: cmd.Request.ResidenceAddress,
                file: cmd.Request.ResidenceAddressCertificate,
                currentProfileResourceId: current,
                folder: ProfileFileCategories.ResidenceCertificate,
                ct);

            if (newId.IsFailed) return Result.Fail<Unit>(newId.Errors);
            profile.ResidenceAddress.CertificateId = newId.Value;
            if (profile.Status == UserProfileStatus.RequiresUpdate)
                await ReviewItemSaveHelper.UpdateSectionStatusAsync(uow, profile, ProfileSection.Contact, ct);
        }

        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
    }
}
