using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Constants;
using Tawtheef.Application.Features.Operations.Admin.Roles.DTOs;
using Tawtheef.Application.Features.Operations.Admin.Roles.Queries;

namespace Tawtheef.Application.Features.Operations.Admin.Roles.Handlers.Queries;

public sealed class ListPermissionsQueryHandler
    : IRequestHandler<ListPermissionsQuery, IResult<List<PermissionDto>>>
{
    public Task<IResult<List<PermissionDto>>> Handle(ListPermissionsQuery request, CancellationToken cancellationToken)
    {
        var permissions = new List<PermissionDto>
        {
            new(PermissionNames.UsersView, PermissionNames.UsersView),
            new(PermissionNames.UsersManage, PermissionNames.UsersManage),
            new(PermissionNames.OfficesView, PermissionNames.OfficesView),
            new(PermissionNames.OfficesManage, PermissionNames.OfficesManage),
            new(PermissionNames.CountriesView, PermissionNames.CountriesView),
            new(PermissionNames.CountriesManage, PermissionNames.CountriesManage),
            new(PermissionNames.ProfileView, PermissionNames.ProfileView),
            new(PermissionNames.ProfileManage, PermissionNames.ProfileManage),
            new(PermissionNames.JobsView, PermissionNames.JobsView),
            new(PermissionNames.JobsManage, PermissionNames.JobsManage)
        };

        return Task.FromResult<IResult<List<PermissionDto>>>(Result.Ok(permissions));
    }
}
