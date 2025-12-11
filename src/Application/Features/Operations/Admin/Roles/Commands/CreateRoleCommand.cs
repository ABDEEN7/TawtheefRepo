using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Operations.Admin.Roles.DTOs;

namespace Tawtheef.Application.Features.Operations.Admin.Roles.Commands;

public sealed record CreateRoleCommand(string NameAr, string NameEn, IReadOnlyCollection<string> Permissions)
    : IRequest<IResult<RoleDto>>;
