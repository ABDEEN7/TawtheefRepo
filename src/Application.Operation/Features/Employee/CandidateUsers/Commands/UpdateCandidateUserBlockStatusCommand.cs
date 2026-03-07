using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.CandidateUsers.Commands;

public sealed record UpdateCandidateUserBlockStatusCommand : IRequest<IResult<Unit>>
{
    public Guid UserId { get; init; }
    public bool IsBlocked { get; init; }
}

