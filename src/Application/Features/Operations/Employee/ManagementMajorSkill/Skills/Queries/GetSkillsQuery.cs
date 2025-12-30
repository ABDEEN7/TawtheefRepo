using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Common.Models.Pagination;

namespace Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Skills.Queries;

public record GetSkillsQuery(string? Search,Guid? SkillTypeId) : PaginatedRequest, IRequest<IResult<PaginatedResult<DropdownOptions>>>;
