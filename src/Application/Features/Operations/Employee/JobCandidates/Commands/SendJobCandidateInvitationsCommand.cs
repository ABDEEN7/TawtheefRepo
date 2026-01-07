using Cortex.Mediator.Commands;
using FluentResults;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.DTOs;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.Models;

namespace Tawtheef.Application.Features.Operations.Employee.JobCandidates.Commands;

public sealed record SendJobCandidateInvitationsCommand(Guid JobId)
    : ICommand<IResult<SendJobCandidateInvitationsResult>>
{
    public JobCandidatesFilter? Filter { get; init; }
    public IReadOnlyCollection<Guid>? ApplicantIds { get; init; }
}
