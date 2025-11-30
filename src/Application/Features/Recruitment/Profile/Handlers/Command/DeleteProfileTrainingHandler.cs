using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Recruitment.Profile.Command;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Applicant;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command;

public sealed class DeleteProfileTrainingHandler(IUnitOfWork uow) :
    IRequestHandler<DeleteProfileTrainingCommand, IResult<Unit>>
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