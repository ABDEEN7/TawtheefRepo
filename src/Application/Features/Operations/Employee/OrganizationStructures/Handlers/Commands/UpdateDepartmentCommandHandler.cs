using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.Employee.OrganizationStructures.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Application.Features.Operations.Employee.OrganizationStructures.Handlers.Commands;

public sealed class UpdateDepartmentCommandHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateDepartmentCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
    {
        var repo = uow.GetEntityRepository<Department>().DbSet;
        var department = await repo.FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);
        if (department is null)
        {
            return Result.Fail<Unit>(new Error(ErrorsCodes.DepartmentNotFound));
        }

        var managementExists = await uow.GetEntityRepository<Management>().DbSet
            .AnyAsync(m => m.Id == request.ManagementId, cancellationToken);
        if (!managementExists)
        {
            return Result.Fail<Unit>(new Error(ErrorsCodes.ManagementNotFound));
        }

        var duplicate = await repo.AnyAsync(
            d => d.Id != request.Id &&
                 d.ManagementId == request.ManagementId &&
                 (d.NameAr == request.NameAr || d.NameEn == request.NameEn),
            cancellationToken);

        if (duplicate)
        {
            return Result.Fail<Unit>(new Error(ErrorsCodes.DepartmentNameAlreadyExists));
        }

        department.ManagementId = request.ManagementId;
        department.NameAr = request.NameAr;
        department.NameEn = request.NameEn;
        department.DescriptionAr = request.DescriptionAr;
        department.DescriptionEn = request.DescriptionEn;
        department.DisplayOrder = request.DisplayOrder;
        department.IsActive = request.IsActive;

        await uow.SaveChangesAsync(cancellationToken);
        return Result.Ok(Unit.Value);
    }
}
