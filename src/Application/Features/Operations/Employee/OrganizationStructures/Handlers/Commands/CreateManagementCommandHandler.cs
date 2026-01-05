using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.Employee.OrganizationStructures.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Application.Features.Operations.Employee.OrganizationStructures.Handlers.Commands;

public sealed class CreateManagementCommandHandler(IUnitOfWork uow)
    : ICommandHandler<CreateManagementCommand, IResult<Guid>>
{
    public async Task<IResult<Guid>> Handle(CreateManagementCommand request, CancellationToken cancellationToken)
    {
        var sectorExists = await uow.GetEntityRepository<Sector>().DbSet
            .AnyAsync(s => s.Id == request.SectorId, cancellationToken);
        if (!sectorExists)
        {
            return Result.Fail<Guid>(new Error(ErrorsCodes.SectorNotFound));
        }

        var repo = uow.GetEntityRepository<Management>().DbSet;
        var duplicate = await repo.AnyAsync(
            m => m.SectorId == request.SectorId &&
                 (m.NameAr == request.NameAr || m.NameEn == request.NameEn),
            cancellationToken);

        if (duplicate)
        {
            return Result.Fail<Guid>(new Error(ErrorsCodes.ManagementNameAlreadyExists));
        }

        var management = new Management
        {
            SectorId = request.SectorId,
            NameAr = request.NameAr,
            NameEn = request.NameEn,
            DescriptionAr = request.DescriptionAr,
            DescriptionEn = request.DescriptionEn,
            IsActive = request.IsActive,
            DisplayOrder = request.DisplayOrder,
            BackendName = $"MGMT_{Guid.NewGuid():N}"[..16].ToUpper()
        };

        await uow.GetEntityRepository<Management>().AddAsync(management);
        await uow.SaveChangesAsync(cancellationToken);

        return Result.Ok(management.Id);
    }
}
