using MediatR;
using FluentResults;

namespace Application.Operation.Features.Admin.Offices.Commands;

public sealed record BlockOfficeUserCommand(Guid OfficeId, Guid UserId, bool IsBlocked)
    : IRequest<IResult<Unit>>;

