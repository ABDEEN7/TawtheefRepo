using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.Admin.Roles.DTOs;
using Tawtheef.Application.Features.Operations.Admin.Roles.Queries;
using Tawtheef.Domain.Entities.Security;

namespace Tawtheef.Application.Features.Operations.Admin.Roles.Handlers.Queries;

public sealed class ListPermissionsQueryHandler(IUnitOfWork uow)
    : IRequestHandler<ListPermissionsQuery, IResult<List<PermissionDto>>>
{
    public async Task<IResult<List<PermissionDto>>> Handle(ListPermissionsQuery request, CancellationToken cancellationToken)
    {
        var permissions = await uow.GetEntityRepository<Permission>().DbSet
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.DisplayOrder)
            .Select(p => new PermissionDto(p.BackendName, p.NameEn))
            .ToListAsync(cancellationToken);

        return Result.Ok(permissions);
    }
}
