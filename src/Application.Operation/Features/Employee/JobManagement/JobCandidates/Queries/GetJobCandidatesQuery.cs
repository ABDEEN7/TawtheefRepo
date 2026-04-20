using Application.Operation.Features.Employee.JobManagement.JobCandidates.DTOs;
using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Queries;

public sealed record GetJobCandidatesQuery(Guid JobId)
    : PaginatedRequest, IRequest<IResult<JobCandidatesCombinedDto>>
{
    public JobCandidatesFilter? Filter { get; init; }
}

