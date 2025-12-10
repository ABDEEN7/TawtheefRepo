using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Operations.Admin.Users.DTOs;

namespace Tawtheef.Application.Features.Operations.Admin.Users.Queries;

public sealed record GetUserRolesQuery(Guid UserId) : IRequest<IResult<UserRoleAssignmentDto>>;
