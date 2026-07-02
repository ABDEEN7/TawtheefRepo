using MediatR;
using FluentResults;

namespace Application.Operation.Features.Admin.Offices.Commands;

public sealed record BlockOfficeUserCommand : IRequest<IResult<Unit>>
{
    public Guid OfficeId { get; init; }
    public Guid UserId { get; init; }
    public bool IsBlocked { get; init; }
}
