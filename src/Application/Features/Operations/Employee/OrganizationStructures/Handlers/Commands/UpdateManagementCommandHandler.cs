using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.Employee.OrganizationStructures.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Application.Features.Operations.Employee.OrganizationStructures.Handlers.Commands;

public sealed class UpdateManagementCommandHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateManagementCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(UpdateManagementCommand request, CancellationToken cancellationToken)
    {
        var managementRepo = uow.GetEntityRepository<Management>().DbSet;
        var management = await managementRepo.FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);
        if (management is null)
        {
            return Result.Fail<Unit>(new Error(ErrorsCodes.ManagementNotFound));
        }

        var sectorExists = await uow.GetEntityRepository<Sector>().DbSet
            .AnyAsync(s => s.Id == request.SectorId, cancellationToken);
        if (!sectorExists)
        {
            return Result.Fail<Unit>(new Error(ErrorsCodes.SectorNotFound));
        }

        var duplicate = await managementRepo.AnyAsync(
            m => m.Id != request.Id &&
                 m.SectorId == request.SectorId &&
                 (m.NameAr == request.NameAr || m.NameEn == request.NameEn),
            cancellationToken);

        if (duplicate)
        {
            return Result.Fail<Unit>(new Error(ErrorsCodes.ManagementNameAlreadyExists));
        }

        management.SectorId = request.SectorId;
        management.NameAr = request.NameAr;
        management.NameEn = request.NameEn;
        management.DescriptionAr = request.DescriptionAr;
        management.DescriptionEn = request.DescriptionEn;
        management.IsActive = request.IsActive;
        management.DisplayOrder = request.DisplayOrder;

        await uow.SaveChangesAsync(cancellationToken);
        return Result.Ok(Unit.Value);
    }
}
