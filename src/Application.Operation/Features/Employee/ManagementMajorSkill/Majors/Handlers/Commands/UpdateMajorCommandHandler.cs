using Application.Operation.Features.Employee.ManagementMajorSkill.Majors.Commands;
using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Application.Operation.Features.Employee.ManagementMajorSkill.Majors.Handlers.Commands;

public class UpdateMajorCommandHandler(IUnitOfWork uow) : IRequestHandler<UpdateMajorCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(UpdateMajorCommand request, CancellationToken cancellationToken)
    {
        var checkResult = await CheckIfMajorUsedAsync(request.Id, cancellationToken);
        if (checkResult.IsFailed) return checkResult;
        
        var majorRepo = uow.GetEntityRepository<Major>();
        var major = await majorRepo.DbSet.FirstOrDefaultAsync(m=> m.Id == request.Id, cancellationToken);
        if (major == null)
            return Result.Fail<Unit>(new Error(ErrorsCodes.MajorNotFound).WithMetadata("MajorId", request.Id));
        
        //check if name ar or name en already exists
        var isNameDuplicated = await uow.GetEntityRepository<Major>().DbSet.AnyAsync(x => x.Id != request.Id && (x.NameAr == request.NameAr || x.NameEn == request.NameEn), cancellationToken);
        if(isNameDuplicated)
            return Result.Fail<Unit>(new Error(ErrorsCodes.MajorNameAlreadyExists));
        
        var result = major.UpdateDetails(request.NameAr, request.NameEn,request.IsActive, request.ParentMajorId);
        if (result.IsFailed) return result;
        
        await majorRepo.UpdateAsync(major);
        await uow.SaveChangesAsync(cancellationToken);

        return Result.Ok(Unit.Value);
    }
    
    private async Task<IResult<Unit>> CheckIfMajorUsedAsync(Guid majorId, CancellationToken cancellationToken)
    {
        var jobApplicationRepo = uow.GetEntityRepository<Tawtheef.Domain.Entities.Recruitment.Job>();
        
        var majorUsedInJobApplications = await jobApplicationRepo.DbSet.AnyAsync(ja => ja.MajorId == majorId || ja.SubMajorId == majorId, cancellationToken);
        if (majorUsedInJobApplications)
            return Result.Fail<Unit>(new Error(ErrorsCodes.MajorAlreadyUsed).WithMetadata("MajorId", majorId));
        
        return Result.Ok(Unit.Value);
    }
}

