using MediatR;
using FluentResults;

namespace Application.Operation.Features.Admin.Offices.Commands;

public sealed record SetOfficeStatusCommand : IRequest<IResult<Unit>>
{
    public Guid OfficeId { get; init; }
    public bool IsActive { get; init; }
}
