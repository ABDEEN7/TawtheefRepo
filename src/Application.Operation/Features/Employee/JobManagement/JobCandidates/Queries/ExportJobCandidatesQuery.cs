using Application.Operation.Features.Employee.JobCandidates.DTOs;
using Application.Operation.Features.Employee.JobCandidates.Models;
using Cortex.Mediator.Queries;
using FluentResults;

namespace Application.Operation.Features.Employee.JobCandidates.Queries;

public sealed record ExportJobCandidatesQuery(Guid JobId)
    : IQuery<IResult<JobCandidatesExportResult>>
{
    public JobCandidatesFilter? Filter { get; init; }
}
