using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Common.Models.Pagination;

namespace Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Majors.Queries;

public record GetSubMajorsQuery(Guid ParentId, string? Search) : PaginatedRequest, IRequest<IResult<PaginatedResult<DropdownOptions>>>;
