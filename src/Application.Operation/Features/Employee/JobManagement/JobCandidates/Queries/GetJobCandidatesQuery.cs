using Application.Operation.Features.Employee.JobManagement.JobCandidates.DTOs;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.Models;
using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Queries;

public sealed record GetJobCandidatesQuery(Guid JobId)
    : PaginatedRequest, IQuery<IResult<JobCandidatesCombinedDto>>
{
    public JobCandidatesFilter? Filter { get; init; }
}
