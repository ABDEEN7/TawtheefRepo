using Application.Operation.Features.Employee.ManagementMajorSkill.Skills.Commands;
using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Application.Operation.Features.Employee.ManagementMajorSkill.Skills.Handlers.Commands;

public sealed class DeleteSkillCommandHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteSkillCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(DeleteSkillCommand request, CancellationToken ct)
    {
        var repo = uow.GetEntityRepository<Skill>();
        var skill = await repo.DbSet.FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (skill is null)
            return Result.Fail<Unit>(new Error(ErrorsCodes.SkillNotFound).WithMetadata("SkillId", request.Id));

        var isUsed = await uow.GetEntityRepository<MajorSkill>()
            .DbSet.AnyAsync(x => x.SkillId == skill.Id, ct);
        
        if (isUsed)
            return Result.Fail<Unit>(new Error(ErrorsCodes.SkillAlreadyUsed).WithMetadata("SkillId", request.Id));

        repo.DbSet.Remove(skill);
        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
    }
}

