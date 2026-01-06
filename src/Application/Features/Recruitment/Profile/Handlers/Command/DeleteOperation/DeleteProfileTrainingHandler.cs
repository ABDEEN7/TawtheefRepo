using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Recruitment.Profile.Command.DeleteOperation;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Applicant;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command.DeleteOperation;

public sealed class DeleteProfileTrainingHandler(IUnitOfWork uow) :
    ICommandHandler<DeleteProfileTrainingCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(DeleteProfileTrainingCommand cmd, CancellationToken ct)
    {
        var repo = uow.GetEntityRepository<TrainingCourse>();
        var target = await repo.DbSet
            .FirstOrDefaultAsync(x => x.Id == cmd.TrainingId, ct);

        if (target is null)
            return Result.Fail<Unit>(ErrorsCodes.TrainingNotFound);

        await repo.DeleteAsync(target);
        await uow.SaveChangesAsync(ct);

        return Result.Ok(Unit.Value);
    }
}
