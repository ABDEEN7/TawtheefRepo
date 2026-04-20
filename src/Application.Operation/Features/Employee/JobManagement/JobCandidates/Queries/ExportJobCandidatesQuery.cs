using Application.Operation.Features.Employee.JobManagement.JobCandidates.DTOs;
using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Queries;

public sealed record ExportJobCandidatesQuery(Guid JobId)
    : IRequest<IResult<JobCandidatesExportResult>>
{
    public JobCandidatesFilter? Filter { get; init; }
}

