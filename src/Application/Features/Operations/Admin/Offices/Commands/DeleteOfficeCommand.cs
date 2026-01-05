using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Tawtheef.Application.Features.Operations.Admin.Offices.Commands;

public sealed record DeleteOfficeCommand(Guid UserId,Guid OfficeId) : ICommand<IResult<Unit>>;
