using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Skills.DTOs;

namespace Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Skills.Queries;

public record GetSkillsQuery(string? Search,Guid? SkillTypeId) : PaginatedRequest, IQuery<IResult<PaginatedResult<SkillDetailsDto>>>;
