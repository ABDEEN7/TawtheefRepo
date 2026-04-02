using MediatR;
using FluentResults;

namespace Tawtheef.Application.Features.Notifications.Commands;

public sealed record UpdateManyNotificationsStateCommand(Guid UserId, List<Guid> NotificationIds, NotificationAction Action)
    : IRequest<IResult<Unit>>;
