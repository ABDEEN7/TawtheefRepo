using Application.Operation.Features.Employee.ManagementMajorSkill.Majors.Commands;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Application.Operation.Features.Employee.ManagementMajorSkill.Majors.Handlers.Commands;

public class DeleteMajorCommandHandler(IUnitOfWork uow) : ICommandHandler<DeleteMajorCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(DeleteMajorCommand request, CancellationToken cancellationToken)
    {
        var majorRepo = uow.GetEntityRepository<Major>();
        var major = await majorRepo.DbSet.AsNoTracking().FirstOrDefaultAsync(m=> m.Id == request.Id, cancellationToken);
        if (major == null)
            return Result.Fail<Unit>(new Error(ErrorsCodes.MajorNotFound).WithMetadata("MajorId", request.Id));
        
        var isUsed = await uow.GetEntityRepository<MajorSkill>()
            .DbSet.AnyAsync(x => x.MajorId == major.Id, cancellationToken);
        
        if (isUsed)
            return Result.Fail<Unit>(new Error(ErrorsCodes.MajorAlreadyUsed).WithMetadata("MajorId", request.Id));

        await majorRepo.DeleteAsync(major);
        await uow.SaveChangesAsync(cancellationToken);
        return Result.Ok(Unit.Value);
    }
}
