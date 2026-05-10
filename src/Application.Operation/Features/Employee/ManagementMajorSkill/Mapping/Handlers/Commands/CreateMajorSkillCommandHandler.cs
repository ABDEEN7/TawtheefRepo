using Application.Operation.Features.Employee.ManagementMajorSkill.Mapping.Commands;
using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Application.Operation.Features.Employee.ManagementMajorSkill.Mapping.Handlers.Commands;


public sealed class CreateMajorSkillCommandHandler(IUnitOfWork uow)
    : IRequestHandler<CreateMajorSkillCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(CreateMajorSkillCommand request, CancellationToken ct)
    {
        var majorRepo = uow.GetEntityRepository<Major>();
        var skillRepo = uow.GetEntityRepository<Skill>();
        var linkRepo  = uow.GetEntityRepository<MajorSkill>();

        var major = await majorRepo.DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.MajorId && !x.IsDeleted, ct);

        if (major is null)
            return Result.Fail<Unit>(new Error(ErrorsCodes.MajorNotFound).WithMetadata("MajorId", request.MajorId));

        var skill = await skillRepo.DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.SkillId && !x.IsDeleted, ct);

        if (skill is null)
            return Result.Fail<Unit>(new Error(ErrorsCodes.SkillNotFound).WithMetadata("SkillId", request.SkillId));

        // avoid creating active link with inactive major/skill
        if (request.IsActive && (!major.IsActive || !skill.IsActive))
            return Result.Fail<Unit>(new Error(ErrorsCodes.CannotLinkInactiveMajorOrSkill)
                .WithMetadata("MajorId", major.Id)
                .WithMetadata("MajorIsActive", major.IsActive)
                .WithMetadata("SkillId", skill.Id)
                .WithMetadata("SkillIsActive", skill.IsActive)
            );

        var exists = await linkRepo.DbSet
            .AsNoTracking()
            .AnyAsync(x => x.MajorId == request.MajorId &&x.SkillId == request.SkillId, ct);

        if (exists)
            return Result.Fail<Unit>(new Error(ErrorsCodes.MajorSkillLinkAlreadyExists)
                .WithMetadata(nameof(request.MajorId), request.MajorId)
                .WithMetadata(nameof(request.SkillId), request.SkillId)
            );

        var link = new MajorSkill
        {
            MajorId = request.MajorId,
            SkillId = request.SkillId,
            IsActive = request.IsActive
        };

        await linkRepo.AddAsync(link, ct);
        await uow.SaveChangesAsync(ct);

        return Result.Ok(Unit.Value);
    }
}

