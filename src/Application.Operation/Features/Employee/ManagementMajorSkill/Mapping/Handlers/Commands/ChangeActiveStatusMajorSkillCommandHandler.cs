using Application.Operation.Features.Employee.ManagementMajorSkill.Mapping.Commands;
using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Application.Operation.Features.Employee.ManagementMajorSkill.Mapping.Handlers.Commands;

public sealed class ChangeActiveStatusMajorSkillCommandHandler(IUnitOfWork uow)
    : IRequestHandler<ChangeActiveStatusMajorSkillCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(ChangeActiveStatusMajorSkillCommand request, CancellationToken ct)
    {
        var link = await GetLinkAsync(request.Id, ct);
        if (link is null)
            return Fail(ErrorsCodes.MajorSkillLinkNotFound, "MajorSkillLinkId", request.Id);

        if (!request.IsActive)
            return await DeactivateAsync(link, ct);

        var majorResult = await ValidateMajorForActivationAsync(link.MajorId, ct);
        if (majorResult.IsFailed)
            return majorResult;

        var skillResult = await ValidateSkillForActivationAsync(link.SkillId, ct);
        if (skillResult.IsFailed)
            return skillResult;

        link.IsActive = true;
        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
    }

    private Task<MajorSkill?> GetLinkAsync(Guid linkId, CancellationToken ct)
        => uow.GetEntityRepository<MajorSkill>().DbSet.FirstOrDefaultAsync(x => x.Id == linkId, ct);

    private async Task<IResult<Unit>> DeactivateAsync(MajorSkill link, CancellationToken ct)
    {
        link.IsActive = false;
        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
    }

    private async Task<IResult<Unit>> ValidateMajorForActivationAsync(Guid majorId, CancellationToken ct)
    {
        var majorRepo = uow.GetEntityRepository<Major>();
        var major = await majorRepo.DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == majorId, ct);

        if (major is null)
            return Fail(ErrorsCodes.MajorNotFound, "MajorId", majorId);

        if (!major.IsActive)
            return Fail(ErrorsCodes.CannotActivateMajorSkillLinkBecauseMajorIsInactive, "MajorId", major.Id);

        if (!major.ParentId.HasValue)
            return Result.Ok(Unit.Value);

        var parentId = major.ParentId.Value;

        var parent = await majorRepo.DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == parentId, ct);

        if (parent is null)
            return Fail(ErrorsCodes.MajorNotFound, "MajorId", parentId);

        if (!parent.IsActive)
            return Fail(ErrorsCodes.CannotActivateMajorSkillLinkBecauseParentMajorIsInactive, "MajorId", parent.Id);

        return Result.Ok(Unit.Value);
    }

    private async Task<IResult<Unit>> ValidateSkillForActivationAsync(Guid skillId, CancellationToken ct)
    {
        var skillRepo = uow.GetEntityRepository<Skill>();
        var skill = await skillRepo.DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == skillId, ct);

        if (skill is null)
            return Fail(ErrorsCodes.SkillNotFound, "SkillId", skillId);

        if (!skill.IsActive)
            return Fail(ErrorsCodes.CannotActivateMajorSkillLinkBecauseSkillIsInactive, "SkillId", skill.Id);

        return Result.Ok(Unit.Value);
    }

    private static IResult<Unit> Fail(string code, string metaKey, object metaValue)
        => Result.Fail<Unit>(new Error(code).WithMetadata(metaKey, metaValue));
}

