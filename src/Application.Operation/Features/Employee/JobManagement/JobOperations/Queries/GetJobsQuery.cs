using Application.Operation.Features.Employee.JobManagement.JobOperations.DTOs;
using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Employee.JobManagement.JobOperations.Queries;

public record GetJobsQuery(JobQueryFilter? Filter, PaginatedRequest Pagination)
    : IRequest<IResult<PaginatedResult<JobResponseDto>>>;
