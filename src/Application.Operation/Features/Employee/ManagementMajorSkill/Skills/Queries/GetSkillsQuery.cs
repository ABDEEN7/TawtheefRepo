using Application.Operation.Features.Employee.ManagementMajorSkill.Skills.DTOs;
using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Employee.ManagementMajorSkill.Skills.Queries;

public record GetSkillsQuery(string? Search,Guid? SkillTypeId) : PaginatedRequest, IRequest<IResult<PaginatedResult<SkillDetailsDto>>>;

