using MediatR;
using FluentResults;

namespace Application.Operation.Features.Admin.Cities.Commands;

public sealed record UpdateCityStatusCommand : IRequest<IResult<Unit>>
{
    public Guid CityId { get; init; }
    public bool IsActive { get; init; }
}
