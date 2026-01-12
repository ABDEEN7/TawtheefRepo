using Application.Operation.Features.Employee.ManagementMajorSkill.Mapping.DTOs;
using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Employee.ManagementMajorSkill.Mapping.Queries;


public record GetMajorSkillsQuery(string? Search, Guid? ParentMajorId, Guid? SubMajorId, Guid? SkillTypeId, bool? IsActive) 
    : PaginatedRequest, IQuery<IResult<PaginatedResult<MajorSkillListItemDto>>>;
