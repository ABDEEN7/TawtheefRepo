using Application.Operation.Features.Admin.Roles.DTOs;
using Application.Operation.Features.Admin.Roles.Queries;
using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Domain.Entities.Security;

namespace Application.Operation.Features.Admin.Roles.Handlers.Queries;

public sealed class ListPermissionsQueryHandler(IUnitOfWork uow, ILocalizationService localizationService)
    : IRequestHandler<ListPermissionsQuery, IResult<List<PermissionDto>>>
{
    public async Task<IResult<List<PermissionDto>>> Handle(ListPermissionsQuery request, CancellationToken cancellationToken)
    {
        var permissions = await uow.GetEntityRepository<Permission>().DbSet
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.DisplayOrder)
            .ToListAsync(cancellationToken);

        var localized = permissions
            .Select(p =>
            {
                var name = localizationService.GetLocalizedName(p);
                var module = name.Split(" - ")[0].Trim();
                return new PermissionDto(p.BackendName, name, module);
            })
            .ToList();

        return Result.Ok(localized);
    }
}

