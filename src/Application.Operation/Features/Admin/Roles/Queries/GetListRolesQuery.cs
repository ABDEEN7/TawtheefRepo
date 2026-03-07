using Application.Operation.Features.Admin.Roles.DTOs;
using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Admin.Roles.Queries;

public sealed record GetListRolesQuery : PaginatedRequest, IRequest<IResult<PaginatedResult<RoleDto>>>;

