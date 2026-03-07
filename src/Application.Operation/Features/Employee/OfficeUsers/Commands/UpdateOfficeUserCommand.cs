using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.OfficeUsers.Commands;

public sealed record UpdateOfficeUserCommand : IRequest<IResult<Unit>>
{
    public Guid UserId { get; init; }
    public required string NameAr { get; init; }
    public required string NameEn { get; init; }
    public required string Email { get; init; }
}

