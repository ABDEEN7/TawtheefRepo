using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Operations.Admin.Roles.DTOs;

namespace Tawtheef.Application.Features.Operations.Admin.Roles.Commands;

public sealed record UpdateRoleCommand(
    Guid Id,
    string NameAr,
    string NameEn,
    string? DescriptionAr,
    string? DescriptionEn,
    IReadOnlyCollection<string> Permissions)
    : IRequest<IResult<RoleDto>>;
