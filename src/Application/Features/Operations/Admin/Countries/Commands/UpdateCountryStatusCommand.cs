using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Tawtheef.Application.Features.Operations.Admin.Countries.Commands;

public sealed record UpdateCountryStatusCommand(Guid CountryId, bool IsActive)
    : ICommand<IResult<Unit>>;
