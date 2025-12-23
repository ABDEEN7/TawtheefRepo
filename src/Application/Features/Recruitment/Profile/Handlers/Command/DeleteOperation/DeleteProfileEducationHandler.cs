using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Recruitment.Profile.Command.DeleteOperation;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Applicant;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command.DeleteOperation;

public sealed class DeleteProfileEducationHandler(IUnitOfWork uow) :
    IRequestHandler<DeleteProfileEducationCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(DeleteProfileEducationCommand cmd, CancellationToken ct)
    {
        var repo = uow.GetEntityRepository<Qualification>();
        var target = await repo.DbSet
            .FirstOrDefaultAsync(x => x.Id == cmd.DegreeId, ct);

        if (target is null)
            return Result.Fail<Unit>(ErrorsCodes.DegreeNotFound);

        var experiencesRepo = uow.GetEntityRepository<Experience>();
        var isLinkedToExperience = await experiencesRepo.DbSet
            .AnyAsync(x => x.QualificationId == target.Id, ct);

        if (isLinkedToExperience)
            return Result.Fail<Unit>(ErrorsCodes.DegreeLinkedToExperience);

        await repo.DeleteAsync(target);
        await uow.SaveChangesAsync(ct);

        return Result.Ok(Unit.Value);
    }
}