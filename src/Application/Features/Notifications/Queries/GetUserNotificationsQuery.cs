using MediatR;
using FluentResults;
using Tawtheef.Application.Features.Notifications.DTOs;

namespace Tawtheef.Application.Features.Notifications.Queries;

public sealed record GetUserNotificationsQuery(Guid UserId, bool UnreadOnly = false, int Limit = 10, DateTimeOffset? CreatedDateBefore = null)
    : IRequest<IResult<IReadOnlyList<UserNotificationDto>>>;

