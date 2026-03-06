using Application.Operation.Features.Employee.OrganizationStructures.Commands;
using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Employee.OrganizationStructures.Handlers.Commands;

public sealed class ChangeSectorActivationCommandHandler(IUnitOfWork uow)
    : IRequestHandler<ChangeSectorActivationCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(ChangeSectorActivationCommand request, CancellationToken cancellationToken)
    {
        var repo = uow.GetEntityRepository<Sector>().DbSet;
        var sector = await repo.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (sector is null)
        {
            return Result.Fail<Unit>(new Error(ErrorsCodes.SectorNotFound));
        }

        sector.IsActive = request.IsActive;

        if (request.ApplyOnHierarchy)
        {
            var managementIds = uow.GetEntityRepository<Management>().DbSet
                .Where(m => m.SectorId == sector.Id)
                .Select(m => m.Id);

            await uow.GetEntityRepository<Management>().DbSet
                .Where(m => m.SectorId == sector.Id)
                .ExecuteUpdateAsync(s => s.SetProperty(m => m.IsActive, request.IsActive), cancellationToken);

            await uow.GetEntityRepository<Department>().DbSet
                .Where(d => managementIds.Contains(d.ManagementId))
                .ExecuteUpdateAsync(s => s.SetProperty(d => d.IsActive, request.IsActive), cancellationToken);
        }

        await uow.SaveChangesAsync(cancellationToken);
        return Result.Ok(Unit.Value);
    }
}

