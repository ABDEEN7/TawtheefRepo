using Cortex.Mediator.Commands;
using FluentResults;
using Tawtheef.Application.Features.Operations.Admin.Roles.DTOs;

namespace Tawtheef.Application.Features.Operations.Admin.Roles.Commands;

public sealed record UpdateRoleCommand(
    Guid Id,
    string NameAr,
    string NameEn,
    string? DescriptionAr,
    string? DescriptionEn,
    IReadOnlyCollection<string> Permissions)
    : ICommand<IResult<RoleDto>>;
