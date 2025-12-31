using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.DTOs;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.Models;

namespace Tawtheef.Application.Features.Operations.Employee.JobCandidates.Queries;

public sealed record ExportJobCandidatesQuery(Guid JobId)
    : IRequest<IResult<JobCandidatesExportResult>>
{
    public JobCandidatesFilter? Filter { get; init; }
    public IReadOnlyCollection<Guid>? InvitationIds { get; init; }
}
