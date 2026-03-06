using MediatR;
using FluentResults;

namespace Application.Operation.Features.Admin.Countries.Commands;

public sealed record UpdateCountryStatusCommand(Guid CountryId, bool IsActive)
    : IRequest<IResult<Unit>>;

