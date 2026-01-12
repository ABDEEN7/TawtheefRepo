using Application.Operation.Features.Employee.JobManagement.JobCandidates.DTOs;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.Models;
using Cortex.Mediator.Queries;
using FluentResults;

namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Queries;

public sealed record ExportJobCandidatesQuery(Guid JobId)
    : IQuery<IResult<JobCandidatesExportResult>>
{
    public JobCandidatesFilter? Filter { get; init; }
}
