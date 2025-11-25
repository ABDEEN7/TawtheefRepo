using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Recruitment.Profile.Command;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command;


public sealed class SaveProfileAttachmentsHandler(
    IUnitOfWork uow
) : IRequestHandler<SaveProfileAttachmentsCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(SaveProfileAttachmentsCommand cmd, CancellationToken ct)
    {
        var profileRepo = uow.GetEntityRepository<UserProfile>();
        var attachRepo  = uow.GetEntityRepository<ProfileAdditionalAttachment>();

        var profile = await profileRepo.DbSet
            .Include(p => p.AdditionalAttachments)
            .FirstOrDefaultAsync(p => p.UserId == cmd.UserId, ct);

        if (profile is null)
        {
            profile = new UserProfile
            {
                UserId  = cmd.UserId,
                IsDraft = true
            };
            await profileRepo.AddAsync(profile);
            await uow.SaveChangesAsync(ct);
        }

        if (profile.AdditionalAttachments is not null && profile.AdditionalAttachments.Count > 0)
        {
            attachRepo.DbSet.RemoveRange(profile.AdditionalAttachments);
        }

        profile.AdditionalAttachments = cmd.Request.Attachments
            .Select(a => new ProfileAdditionalAttachment
            {
                FileName      = a.FileName,
                AttachmentId  = a.AttachmentId,
                UserProfileId = profile.Id
            })
            .ToList();

        profile.IsDraft = !cmd.Request.Submit;

        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
    }
}
