using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Operation.Features.Admin.Countries.Commands;

public sealed record UpdateCountryStatusCommand(Guid CountryId, bool IsActive)
    : ICommand<IResult<Unit>>;
