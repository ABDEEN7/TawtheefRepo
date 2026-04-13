using MediatR;
using FluentResults;

namespace Application.Operation.Features.Admin.Cities.Commands;

public sealed record UpdateCityCommand : IRequest<IResult<Unit>>
{
    public Guid Id { get; init; }
    public Guid CountryId { get; init; }
    public string NameAr { get; init; } = string.Empty;
    public string NameEn { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public bool IsActive { get; init; }
}
