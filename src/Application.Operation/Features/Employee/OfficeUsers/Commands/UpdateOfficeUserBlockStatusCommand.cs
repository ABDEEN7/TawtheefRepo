using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.OfficeUsers.Commands;

public sealed record UpdateOfficeUserBlockStatusCommand : IRequest<IResult<Unit>>
{
    public Guid UserId { get; init; }
    public bool IsBlocked { get; init; }
}

