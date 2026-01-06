using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.DTOs;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.Models;

namespace Tawtheef.Application.Features.Operations.Employee.JobCandidates.Queries;

public sealed record ExportJobCandidatesQuery(Guid JobId)
    : IQuery<IResult<JobCandidatesExportResult>>
{
    public JobCandidatesFilter? Filter { get; init; }
    public IReadOnlyCollection<Guid>? InvitationIds { get; init; }
}
