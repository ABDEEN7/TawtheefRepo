using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.Employee.OrganizationStructures.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Application.Features.Operations.Employee.OrganizationStructures.Handlers.Commands;

public sealed class ChangeDepartmentActivationCommandHandler(IUnitOfWork uow)
    : IRequestHandler<ChangeDepartmentActivationCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(ChangeDepartmentActivationCommand request, CancellationToken cancellationToken)
    {
        var repo = uow.GetEntityRepository<Department>().DbSet;
        var department = await repo.FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

        if (department is null)
        {
            return Result.Fail<Unit>(new Error(ErrorsCodes.DepartmentNotFound));
        }

        department.IsActive = request.IsActive;

        await uow.SaveChangesAsync(cancellationToken);
        return Result.Ok(Unit.Value);
    }
}
