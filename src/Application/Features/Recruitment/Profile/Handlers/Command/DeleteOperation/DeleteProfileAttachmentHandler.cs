using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Recruitment.Profile.Command;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Applicant;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command.DeleteOperation;


public sealed class DeleteProfileAttachmentHandler(IUnitOfWork uow) :
    IRequestHandler<DeleteProfileAttachmentCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(DeleteProfileAttachmentCommand cmd, CancellationToken ct)
    {
        var repo = uow.GetEntityRepository<ProfileAdditionalAttachment>();
        var target = await repo.DbSet
            .FirstOrDefaultAsync(x => x.Id == cmd.AttachmentId, ct);

        if (target is null)
            return Result.Fail<Unit>(ErrorsCodes.AttachmentNotFound);

        await repo.DeleteAsync(target);
        await uow.SaveChangesAsync(ct);

        return Result.Ok(Unit.Value);
    }
}
