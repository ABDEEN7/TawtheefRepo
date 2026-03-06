using Application.Operation.Features.Employee.ManagementMajorSkill.Skills.Commands;
using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Application.Operation.Features.Employee.ManagementMajorSkill.Skills.Handlers.Commands;

public sealed class UpdateSkillCommandHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateSkillCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(UpdateSkillCommand request, CancellationToken cancellationToken)
    {
        var checkResult = await CheckIfSkillUsedAsync(request.Id, cancellationToken);
        if (checkResult.IsFailed) return checkResult;
        
        var skillRepo = uow.GetEntityRepository<Skill>();
        var skill = await skillRepo.DbSet.FirstOrDefaultAsync(m=> m.Id == request.Id, cancellationToken);
        if (skill == null)
            return Result.Fail<Unit>(new Error(ErrorsCodes.MajorNotFound).WithMetadata("SkillId", request.Id));
        
        //check if name ar or name en already exists
        var isNameDuplicated = await uow.GetEntityRepository<Skill>().DbSet.AnyAsync(x => x.Id != request.Id && (x.NameAr == request.NameAr || x.NameEn == request.NameEn), cancellationToken);
        if(isNameDuplicated)
            return Result.Fail<Unit>(new Error(ErrorsCodes.MajorNameAlreadyExists));

        skill.UpdateDetails(request.NameAr, request.NameEn, 
            request.DescriptionAr, request.DescriptionEn, 
            request.SkillTypeId, request.IsActive);
        
        await skillRepo.UpdateAsync(skill);
        await uow.SaveChangesAsync(cancellationToken);

        return Result.Ok(Unit.Value);
    }
    
    private async Task<IResult<Unit>> CheckIfSkillUsedAsync(Guid skillId, CancellationToken cancellationToken)
    {
        var jobApplicationRepo = uow.GetEntityRepository<JobSkill>();
        
        var skillUsedInJobApplications = await jobApplicationRepo.DbSet.AnyAsync(ja => ja.SkillId == skillId, cancellationToken);
        if (skillUsedInJobApplications)
            return Result.Fail<Unit>(new Error(ErrorsCodes.SkillAlreadyUsed).WithMetadata("SkillId", skillId));
        
        return Result.Ok(Unit.Value);
    }
}

