using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Operation.Features.Employee.OfficeUsers.Commands;

public sealed record UpdateOfficeUserCommand : ICommand<IResult<Unit>>
{
    public Guid UserId { get; init; }
    public string NameAr { get; init; } = string.Empty;
    public string NameEn { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}
