using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Operation.Features.Employee.OfficeUsers.Commands;

public sealed record UpdateOfficeUserCommand : ICommand<IResult<Unit>>
{
    public Guid UserId { get; init; }
    public required string NameAr { get; init; }
    public required string NameEn { get; init; }
    public required string Email { get; init; }
}
