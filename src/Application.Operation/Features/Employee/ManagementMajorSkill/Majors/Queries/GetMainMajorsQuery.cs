using Application.Operation.Features.Employee.ManagementMajorSkill.Majors.DTOs;
using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Employee.ManagementMajorSkill.Majors.Queries;

public record GetMainMajorsQuery(string? Search) 
    : PaginatedRequest, IRequest<IResult<PaginatedResult<MajorDetailsDto>>>;

