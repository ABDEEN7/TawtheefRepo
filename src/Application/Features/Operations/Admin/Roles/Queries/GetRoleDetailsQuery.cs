using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Features.Operations.Admin.Roles.DTOs;

namespace Tawtheef.Application.Features.Operations.Admin.Roles.Queries;

public sealed record GetRoleDetailsQuery(Guid Id) : IQuery<IResult<RoleDto>>;
