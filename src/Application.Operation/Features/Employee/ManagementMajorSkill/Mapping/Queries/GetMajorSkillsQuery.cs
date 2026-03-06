using Application.Operation.Features.Employee.ManagementMajorSkill.Mapping.DTOs;
using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Employee.ManagementMajorSkill.Mapping.Queries;


public record GetMajorSkillsQuery(string? Search, Guid? ParentMajorId, Guid? SubMajorId, Guid? SkillTypeId, bool? IsActive) 
    : PaginatedRequest, IRequest<IResult<PaginatedResult<MajorSkillListItemDto>>>;

