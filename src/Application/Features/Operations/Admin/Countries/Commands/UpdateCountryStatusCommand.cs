using System;
using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Operations.Admin.Countries.Commands;

public sealed record UpdateCountryStatusCommand(Guid CountryId, bool IsActive)
    : IRequest<IResult<Unit>>;
