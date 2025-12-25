using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Operations.Admin.Offices.Commands;

public sealed record BlockOfficeUserCommand(Guid OfficeId, Guid UserId, bool IsBlocked)
    : IRequest<IResult<Unit>>;
