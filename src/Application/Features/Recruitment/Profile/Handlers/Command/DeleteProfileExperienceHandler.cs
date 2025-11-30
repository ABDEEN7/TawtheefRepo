using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Recruitment.Profile.Command;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Applicant;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command;

public sealed class DeleteProfileExperienceHandler(IUnitOfWork uow) :
    IRequestHandler<DeleteProfileExperienceCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(DeleteProfileExperienceCommand cmd, CancellationToken ct)
    {
        var repo = uow.GetEntityRepository<Experience>();
        var target = await repo.DbSet
            .FirstOrDefaultAsync(x => x.Id == cmd.ExperienceId, ct);

        if (target is null)
            return Result.Fail<Unit>(ErrorsCodes.ExperienceNotFound);

        await repo.DeleteAsync(target);
        await uow.SaveChangesAsync(ct);

        return Result.Ok(Unit.Value);
    }
}
