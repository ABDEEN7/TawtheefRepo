using Application.Operation.Features.Employee.ManagementMajorSkill.Majors.Commands;
using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Application.Operation.Features.Employee.ManagementMajorSkill.Majors.Handlers.Commands;

public class ChangeActiveStatusMajorCommandHandler(IUnitOfWork uow) : IRequestHandler<ChangeActiveStatusMajorCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(ChangeActiveStatusMajorCommand request, CancellationToken cancellationToken)
    {
        var major = await uow.GetEntityRepository<Major>().DbSet.FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);
        if (major == null)
            return Result.Fail<Unit>(new Error(ErrorsCodes.MajorNotFound).WithMetadata("MajorId", request.Id));

        major.ChangeActiveStatus(request.IsActive);
        
        // Deactivating main Major: deactivate all SubMajors and unlink Majorâ€“Skill (no child confirmation required)
        // Activating main Major: activate all SubMajors and link Majorâ€“Skill (child confirmation required)
        if (!request.IsActive || request.ApplyOnRelationship)
        {
            await uow.GetEntityRepository<Major>().DbSet.Where(m => m.ParentId == major.Id)
                .ExecuteUpdateAsync(s => s.SetProperty(m => m.IsActive, request.IsActive), cancellationToken);
            
            await uow.GetEntityRepository<MajorSkill>().DbSet.Where(ms => ms.MajorId == major.Id)
                .ExecuteUpdateAsync(s => s.SetProperty(ms => ms.IsActive, request.IsActive), cancellationToken);
        }
        
        await uow.SaveChangesAsync(cancellationToken);
        return Result.Ok(Unit.Value);
    }
}

