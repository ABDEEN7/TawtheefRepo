using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.OfficeUsers.Commands;

public sealed record CreateOfficeUserCommand : IRequest<IResult<Guid>>
{
    public required string NameAr { get; init; }
    public required string NameEn { get; init; }
    public required string Email { get; init; }
}

