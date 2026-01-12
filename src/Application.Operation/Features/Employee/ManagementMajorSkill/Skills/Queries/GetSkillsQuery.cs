using Application.Operation.Features.Employee.ManagementMajorSkill.Skills.DTOs;
using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Employee.ManagementMajorSkill.Skills.Queries;

public record GetSkillsQuery(string? Search,Guid? SkillTypeId) : PaginatedRequest, IQuery<IResult<PaginatedResult<SkillDetailsDto>>>;
