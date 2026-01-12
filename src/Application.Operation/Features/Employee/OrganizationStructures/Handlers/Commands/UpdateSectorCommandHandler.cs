using Application.Operation.Features.Employee.OrganizationStructures.Commands;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Employee.OrganizationStructures.Handlers.Commands;

public sealed class UpdateSectorCommandHandler(IUnitOfWork uow)
    : ICommandHandler<UpdateSectorCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(UpdateSectorCommand request, CancellationToken cancellationToken)
    {
        var repo = uow.GetEntityRepository<Sector>().DbSet;

        var sector = await repo.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        if (sector is null)
        {
            return Result.Fail<Unit>(new Error(ErrorsCodes.SectorNotFound));
        }

        var duplicate = await repo.AnyAsync(
            s => s.Id != request.Id && (s.NameAr == request.NameAr || s.NameEn == request.NameEn),
            cancellationToken);

        if (duplicate)
        {
            return Result.Fail<Unit>(new Error(ErrorsCodes.SectorNameAlreadyExists));
        }

        sector.NameAr = request.NameAr;
        sector.NameEn = request.NameEn;
        sector.DescriptionAr = request.DescriptionAr;
        sector.DescriptionEn = request.DescriptionEn;
        sector.DisplayOrder = request.DisplayOrder;
        sector.IsActive = request.IsActive;

        await uow.SaveChangesAsync(cancellationToken);
        return Result.Ok(Unit.Value);
    }
}
