using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.Employee.OrganizationStructures.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Application.Features.Operations.Employee.OrganizationStructures.Handlers.Commands;

public sealed class CreateDepartmentCommandHandler(IUnitOfWork uow)
    : ICommandHandler<CreateDepartmentCommand, IResult<Guid>>
{
    public async Task<IResult<Guid>> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
    {
        var managementRepo = uow.GetEntityRepository<Management>().DbSet;
        var managementExists = await managementRepo.AnyAsync(m => m.Id == request.ManagementId, cancellationToken);
        if (!managementExists)
        {
            return Result.Fail<Guid>(new Error(ErrorsCodes.ManagementNotFound));
        }

        var repo = uow.GetEntityRepository<Department>().DbSet;
        var duplicate = await repo.AnyAsync(
            d => d.ManagementId == request.ManagementId &&
                 (d.NameAr == request.NameAr || d.NameEn == request.NameEn),
            cancellationToken);

        if (duplicate)
        {
            return Result.Fail<Guid>(new Error(ErrorsCodes.DepartmentNameAlreadyExists));
        }

        var department = new Department
        {
            ManagementId = request.ManagementId,
            NameAr = request.NameAr,
            NameEn = request.NameEn,
            DescriptionAr = request.DescriptionAr,
            DescriptionEn = request.DescriptionEn,
            IsActive = request.IsActive,
            DisplayOrder = request.DisplayOrder,
            BackendName = $"DEPT_{Guid.NewGuid():N}"[..16].ToUpper()
        };

        await uow.GetEntityRepository<Department>().AddAsync(department);
        await uow.SaveChangesAsync(cancellationToken);

        return Result.Ok(department.Id);
    }
}
