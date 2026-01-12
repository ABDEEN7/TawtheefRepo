using Application.Operation.Features.Admin.Users.DTOs;
using Cortex.Mediator.Queries;
using FluentResults;

namespace Application.Operation.Features.Admin.Users.Queries;

public sealed record GetUserRolesQuery(Guid UserId) : IQuery<IResult<UserRoleAssignmentDto>>;
