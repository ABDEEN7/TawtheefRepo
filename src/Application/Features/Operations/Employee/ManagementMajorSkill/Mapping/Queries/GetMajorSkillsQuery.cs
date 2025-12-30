using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Mapping.DTOs;

namespace Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Mapping.Queries;


public record GetMajorSkillsQuery(string? Search, Guid? ParentMajorId, Guid? SubMajorId, Guid? SkillTypeId, bool? IsActive) 
    : PaginatedRequest, IRequest<IResult<PaginatedResult<MajorSkillListItemDto>>>;
