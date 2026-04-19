using Application.Operation.Features.Employee.JobManagement.JobCandidates.DTOs;
using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Commands;

public sealed record SendJobCandidateInvitationsCommand(Guid JobId)
    : IRequest<IResult<SendJobCandidateInvitationsResult>>
{
    public JobCandidatesFilter? Filter { get; init; }
    public IReadOnlyCollection<Guid>? ApplicantIds { get; init; }
}

