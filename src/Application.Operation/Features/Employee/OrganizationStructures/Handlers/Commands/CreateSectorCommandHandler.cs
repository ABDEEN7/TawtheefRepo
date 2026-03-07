using Application.Operation.Features.Employee.OrganizationStructures.Commands;
using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Employee.OrganizationStructures.Handlers.Commands;

public sealed class CreateSectorCommandHandler(IUnitOfWork uow)
    : IRequestHandler<CreateSectorCommand, IResult<Guid>>
{
    public async Task<IResult<Guid>> Handle(CreateSectorCommand request, CancellationToken cancellationToken)
    {
        var repo = uow.GetEntityRepository<Sector>().DbSet;
        var isDuplicate = await repo.AnyAsync(
            s => s.NameAr == request.NameAr || s.NameEn == request.NameEn,
            cancellationToken);

        if (isDuplicate)
        {
            return Result.Fail<Guid>(new Error(ErrorsCodes.SectorNameAlreadyExists));
        }

        var sector = new Sector
        {
            NameAr = request.NameAr,
            NameEn = request.NameEn,
            DescriptionAr = request.DescriptionAr,
            DescriptionEn = request.DescriptionEn,
            IsActive = request.IsActive,
            DisplayOrder = request.DisplayOrder,
            BackendName = $"SECTOR_{Guid.NewGuid():N}"[..16].ToUpper()
        };

        await uow.GetEntityRepository<Sector>().AddAsync(sector);
        await uow.SaveChangesAsync(cancellationToken);
        return Result.Ok(sector.Id);
    }
}

