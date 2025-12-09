using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Operations.Admin.Roles.DTOs;

namespace Tawtheef.Application.Features.Operations.Admin.Roles.Queries;

public sealed record GetRoleDetailsQuery(Guid Id) : IRequest<IResult<RoleDto>>;
