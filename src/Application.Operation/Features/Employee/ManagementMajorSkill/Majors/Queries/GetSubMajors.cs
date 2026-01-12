using Application.Operation.Features.Employee.ManagementMajorSkill.Majors.DTOs;
using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Employee.ManagementMajorSkill.Majors.Queries;

public record GetSubMajorsQuery(Guid ParentMajorId, string? Search) 
    : PaginatedRequest, IQuery<IResult<PaginatedResult<MajorDetailsDto>>>;
