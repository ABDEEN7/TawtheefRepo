using Application.Operation.Features.Employee.ManagementMajorSkill.Skills.Commands;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Application.Operation.Features.Employee.ManagementMajorSkill.Skills.Handlers.Commands;

public sealed class ChangeActiveStatusSkillCommandHandler(IUnitOfWork uow)
    : ICommandHandler<ChangeActiveStatusSkillCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(ChangeActiveStatusSkillCommand request, CancellationToken ct)
    {
        var repo = uow.GetEntityRepository<Skill>();

        var skill = await repo.DbSet
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (skill is null)
            return Result.Fail<Unit>(new Error(ErrorsCodes.SkillNotFound).WithMetadata("SkillId", request.Id));

        skill.ChangeActivation(request.IsActive);

        // Deactivating main Skill: deactivate all unlink Major–Skill (no child confirmation required)
        // Activating main Skill: activate all link Major–Skill (child confirmation required)
        if (!request.IsActive || request.ApplyOnRelationship)
        {
            await uow.GetEntityRepository<MajorSkill>().DbSet.Where(ms => ms.SkillId == skill.Id)
                .ExecuteUpdateAsync(s => s.SetProperty(ms => ms.IsActive, request.IsActive), ct);
        }
        
        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
    }
}
