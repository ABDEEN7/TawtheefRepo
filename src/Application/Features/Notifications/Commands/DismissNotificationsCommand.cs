using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Notifications.Commands;

public sealed record DismissNotificationCommand(Guid UserId, Guid NotificationId) : IRequest<IResult<Unit>>;

public sealed record DismissManyNotificationsCommand(Guid UserId, IReadOnlyCollection<Guid> NotificationIds)
    : IRequest<IResult<Unit>>;

public sealed record DismissAllNotificationsCommand(Guid UserId) : IRequest<IResult<Unit>>;
