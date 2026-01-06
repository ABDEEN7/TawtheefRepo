using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Features.Operations.Admin.Roles.DTOs;

namespace Tawtheef.Application.Features.Operations.Admin.Roles.Queries;

public sealed record GetListRolesQuery : PaginatedRequest, IQuery<IResult<PaginatedResult<RoleDto>>>;
