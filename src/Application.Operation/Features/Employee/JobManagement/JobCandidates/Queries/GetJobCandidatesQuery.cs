using Application.Operation.Features.Employee.JobManagement.JobCandidates.DTOs;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.Models;
using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Queries;

public sealed record GetJobCandidatesQuery(Guid JobId, JobCandidatesFilter? Filter)
    : PaginatedRequest, IRequest<IResult<JobCandidatesCombinedDto>>;
