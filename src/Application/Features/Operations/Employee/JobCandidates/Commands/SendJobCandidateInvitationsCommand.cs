using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.DTOs;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.Models;

namespace Tawtheef.Application.Features.Operations.Employee.JobCandidates.Commands;

public sealed record SendJobCandidateInvitationsCommand(Guid JobId)
    : IRequest<IResult<SendJobCandidateInvitationsResult>>
{
    public JobCandidatesFilter? Filter { get; init; }
    public IReadOnlyCollection<Guid>? InvitationIds { get; init; }
}
