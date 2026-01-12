using Application.Operation.Features.Admin.Roles.DTOs;
using Cortex.Mediator.Queries;
using FluentResults;

namespace Application.Operation.Features.Admin.Roles.Queries;

public sealed record ListPermissionsQuery : IQuery<IResult<List<PermissionDto>>>;
