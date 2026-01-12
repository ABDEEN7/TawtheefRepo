using Application.Operation.Features.Employee.JobCandidates.DTOs;
using Application.Operation.Features.Employee.JobCandidates.Models;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Operation.Features.Employee.JobCandidates.Commands;

public sealed record SendJobCandidateInvitationsCommand(Guid JobId)
    : ICommand<IResult<SendJobCandidateInvitationsResult>>
{
    public JobCandidatesFilter? Filter { get; init; }
    public IReadOnlyCollection<Guid>? ApplicantIds { get; init; }
}
