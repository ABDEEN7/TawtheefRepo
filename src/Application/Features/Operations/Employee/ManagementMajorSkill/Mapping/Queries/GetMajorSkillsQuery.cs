using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Mapping.DTOs;

namespace Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Mapping.Queries;


public record GetMajorSkillsQuery(string? Search, Guid? ParentMajorId, Guid? SubMajorId, Guid? SkillTypeId, bool? IsActive) 
    : PaginatedRequest, IQuery<IResult<PaginatedResult<MajorSkillListItemDto>>>;
