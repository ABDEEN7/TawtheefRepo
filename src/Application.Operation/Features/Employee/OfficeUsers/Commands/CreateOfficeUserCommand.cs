using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Operation.Features.Employee.OfficeUsers.Commands;

public sealed record CreateOfficeUserCommand : ICommand<IResult<Guid>>
{
    public required string NameAr { get; init; }
    public required string NameEn { get; init; }
    public required string Email { get; init; }
}
