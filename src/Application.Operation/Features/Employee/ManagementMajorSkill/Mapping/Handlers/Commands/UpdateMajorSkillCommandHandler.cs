using Application.Operation.Features.Employee.ManagementMajorSkill.Mapping.Commands;
using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Application.Operation.Features.Employee.ManagementMajorSkill.Mapping.Handlers.Commands;

public sealed class UpdateMajorSkillCommandHandler(
    IUnitOfWork uow
) : IRequestHandler<UpdateMajorSkillCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(UpdateMajorSkillCommand request, CancellationToken ct)
    {
        var repo = uow.GetEntityRepository<MajorSkill>();

        var link = await repo.DbSet
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (link is null)
            return Result.Fail<Unit>(new Error(ErrorsCodes.MajorSkillLinkNotFound).WithMetadata("LinkId", request.Id));
        
        link.IsSkillRequired = request.IsSkillRequired;
        link.IsActive = request.IsActive;

        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
    }
}

