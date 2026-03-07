using MediatR;
using FluentResults;
using Tawtheef.Application.Features.Notifications.DTOs;

namespace Tawtheef.Application.Features.Notifications.Queries;

public sealed record GetUserNotificationsQuery(Guid UserId, int Limit = 10)
    : IRequest<IResult<IReadOnlyList<UserNotificationDto>>>;

