using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Common.Models.Pagination;

namespace Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Majors.Queries;

public record GetMainMajorsQuery(string Search) : PaginatedRequest, IRequest<IResult<PaginatedResult<DropdownOptions>>>;
