using Application.Operation.Features.Employee.OrganizationStructures.Commands;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Employee.OrganizationStructures.Handlers.Commands;

public sealed class ChangeManagementActivationCommandHandler(IUnitOfWork uow)
    : ICommandHandler<ChangeManagementActivationCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(ChangeManagementActivationCommand request, CancellationToken cancellationToken)
    {
        var repo = uow.GetEntityRepository<Management>().DbSet;
        var management = await repo.FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

        if (management is null)
        {
            return Result.Fail<Unit>(new Error(ErrorsCodes.ManagementNotFound));
        }

        management.IsActive = request.IsActive;

        if (request.ApplyOnHierarchy)
        {
            await uow.GetEntityRepository<Department>().DbSet
                .Where(d => d.ManagementId == management.Id)
                .ExecuteUpdateAsync(s => s.SetProperty(d => d.IsActive, request.IsActive), cancellationToken);
        }

        await uow.SaveChangesAsync(cancellationToken);
        return Result.Ok(Unit.Value);
    }
}
