using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Notifications.Commands;

public sealed record MarkNotificationAsReadCommand(Guid UserId, Guid NotificationId) : IRequest<IResult<Unit>>;

public sealed record MarkManyNotificationsAsReadCommand(Guid UserId, IReadOnlyCollection<Guid> NotificationIds)
    : IRequest<IResult<Unit>>;

public sealed record MarkAllNotificationsAsReadCommand(Guid UserId) : IRequest<IResult<Unit>>;
