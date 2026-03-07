using Application.Operation.Features.Admin.Roles.DTOs;
using MediatR;
using FluentResults;

namespace Application.Operation.Features.Admin.Roles.Queries;

public sealed record ListRoleLookupsQuery : IRequest<IResult<IReadOnlyCollection<RoleLookupDto>>>;

