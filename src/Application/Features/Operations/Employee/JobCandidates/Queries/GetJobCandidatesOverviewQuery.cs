using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.DTOs;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.Models;

namespace Tawtheef.Application.Features.Operations.Employee.JobCandidates.Queries;

public sealed record GetJobCandidatesOverviewQuery(Guid JobId)
    : IRequest<IResult<JobCandidatesOverviewDto>>
{
    public string? SearchTerm { get; init; }
    public Guid? JobCategoryId { get; init; }
    public Guid? CandidateTypeId { get; init; }
    public int? MinimumPoints { get; init; }

    public JobCandidatesFilter ToFilter() =>
        new(SearchTerm, JobCategoryId, CandidateTypeId, MinimumPoints);
}
