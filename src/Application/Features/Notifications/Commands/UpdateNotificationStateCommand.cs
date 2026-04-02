using MediatR;
using FluentResults;

namespace Tawtheef.Application.Features.Notifications.Commands;

public enum NotificationAction
{
    MarkAsRead = 1,
    MarkAsUnread = 2,
    Dismiss = 3
}

public sealed record UpdateNotificationStateCommand(Guid UserId, Guid NotificationId, NotificationAction Action)
    : IRequest<IResult<Unit>>;
