using MediatR;
using FluentResults;

namespace Tawtheef.Application.Features.Notifications.Commands;

public sealed record UpdateAllNotificationsStateCommand(Guid UserId, NotificationAction Action)
    : IRequest<IResult<Unit>>;
