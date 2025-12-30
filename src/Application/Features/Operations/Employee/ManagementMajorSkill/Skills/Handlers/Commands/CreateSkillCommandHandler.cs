using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Skills.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Skills.Handlers.Commands;

public sealed class CreateSkillCommandHandler(IUnitOfWork uow)
    : IRequestHandler<CreateSkillCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(CreateSkillCommand request, CancellationToken ct)
    {
        var repo = uow.GetEntityRepository<Skill>();

        //check if name ar or name en already exists
        var isNameDuplicated = await uow.GetEntityRepository<Skill>().DbSet
            .AnyAsync(x => (x.NameAr == request.NameAr || x.NameEn == request.NameEn) && x.SkillTypeId == request.SkillTypeId, ct);
        if(isNameDuplicated)
            return Result.Fail<Unit>(new Error(ErrorsCodes.MajorNameAlreadyExists));

        var code = "SKILL-" + Guid.NewGuid().ToString("N")[..8].ToUpper();
        await repo.AddAsync(new Skill
        {
            NameAr = request.NameAr,
            NameEn = request.NameEn,
            IsActive = request.IsActive,
            SkillTypeId = request.SkillTypeId,
            BackendName = code,
            DescriptionEn = request.DescriptionEn,
            DescriptionAr = request.DescriptionAr
        });
        await uow.SaveChangesAsync(ct);

        return Result.Ok(Unit.Value);
    }
}
