using Application.Operation.Features.Admin.Roles.DTOs;
using MediatR;
using FluentResults;

namespace Application.Operation.Features.Admin.Roles.Commands;

public sealed record UpdateRoleCommand(
    Guid Id,
    string NameAr,
    string NameEn,
    string? DescriptionAr,
    string? DescriptionEn,
    IReadOnlyCollection<string> Permissions)
    : IRequest<IResult<RoleDto>>;

