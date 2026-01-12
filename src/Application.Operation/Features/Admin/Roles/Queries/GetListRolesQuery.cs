using Application.Operation.Features.Admin.Roles.DTOs;
using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Admin.Roles.Queries;

public sealed record GetListRolesQuery : PaginatedRequest, IQuery<IResult<PaginatedResult<RoleDto>>>;
