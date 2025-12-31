using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.DTOs;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.Models;

namespace Tawtheef.Application.Features.Operations.Employee.JobCandidates.Queries;

public sealed record GetJobCandidatesQuery(Guid JobId)
    : IRequest<IResult<PaginatedResult<JobCandidateListItemDto>>>
{
    public JobCandidatesFilter? Filter { get; init; }
    public PaginatedRequest Pagination { get; init; } = new();
}
