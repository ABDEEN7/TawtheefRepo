using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Majors.DTOs;

namespace Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Majors.Queries;

public record GetSubMajorsQuery(Guid ParentMajorId, string? Search) 
    : PaginatedRequest, IQuery<IResult<PaginatedResult<MajorDetailsDto>>>;
