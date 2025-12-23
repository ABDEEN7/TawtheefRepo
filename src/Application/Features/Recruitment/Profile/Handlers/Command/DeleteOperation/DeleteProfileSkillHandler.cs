using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Recruitment.Profile.Command.DeleteOperation;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Applicant;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command.DeleteOperation;

public sealed class DeleteProfileSkillHandler(IUnitOfWork uow) :
    IRequestHandler<DeleteProfileSkillCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(DeleteProfileSkillCommand cmd, CancellationToken ct)
    {
        var repo = uow.GetEntityRepository<Experience>();
        var target = await repo.DbSet
            .FirstOrDefaultAsync(x => x.Id == cmd.SkillId, ct);

        if (target is null)
            return Result.Fail<Unit>(ErrorsCodes.SkillNotFound);

        await repo.DeleteAsync(target);
        await uow.SaveChangesAsync(ct);

        return Result.Ok(Unit.Value);
    }
}
