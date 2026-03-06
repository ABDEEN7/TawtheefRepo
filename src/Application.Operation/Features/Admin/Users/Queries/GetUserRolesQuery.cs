using Application.Operation.Features.Admin.Users.DTOs;
using MediatR;
using FluentResults;

namespace Application.Operation.Features.Admin.Users.Queries;

public sealed record GetUserRolesQuery(Guid UserId) : IRequest<IResult<UserRoleAssignmentDto>>;

