using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Features.Operations.Admin.Users.DTOs;

namespace Tawtheef.Application.Features.Operations.Admin.Users.Queries;

public sealed record GetUserRolesQuery(Guid UserId) : IQuery<IResult<UserRoleAssignmentDto>>;
