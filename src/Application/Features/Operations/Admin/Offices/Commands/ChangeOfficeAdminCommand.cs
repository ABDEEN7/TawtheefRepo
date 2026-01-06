using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Tawtheef.Application.Features.Operations.Admin.Offices.Commands;

public sealed record ChangeOfficeAdminCommand(Guid OfficeId, Guid UserId) : ICommand<IResult<Unit>>;
