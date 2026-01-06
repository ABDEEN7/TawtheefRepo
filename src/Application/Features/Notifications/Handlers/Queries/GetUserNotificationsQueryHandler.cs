using Cortex.Mediator.Queries;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Notifications.DTOs;
using Tawtheef.Application.Features.Notifications.Queries;
using Tawtheef.Domain.Entities.Notification;

namespace Tawtheef.Application.Features.Notifications.Handlers.Queries;

public sealed class GetUserNotificationsQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetUserNotificationsQuery, IResult<IReadOnlyList<UserNotificationDto>>>
{
    private const int DefaultLimit = 10;
    private const int MaxLimit = 50;

    public async Task<IResult<IReadOnlyList<UserNotificationDto>>> Handle(
        GetUserNotificationsQuery request,
        CancellationToken cancellationToken)
    {
        var safeLimit = Math.Clamp(request.Limit <= 0 ? DefaultLimit : request.Limit, 1, MaxLimit);

        var notifications = await unitOfWork
            .GetEntityRepository<Notification>()
            .DbSet
            .AsNoTracking()
            .Where(n => n.UserId == request.UserId && n.Channel == NotificationChannel.InApp)
            .OrderByDescending(n => n.CreatedDate)
            .Take(safeLimit)
            .Select(n => new UserNotificationDto
            {
                Id = n.Id,
                Subject = n.Subject,
                Body = n.Body,
                Status = n.Status.ToString(),
                CreatedDate = n.CreatedDate,
                SentAtUtc = n.SentAtUtc,
                Error = n.Error
            })
            .ToListAsync(cancellationToken);

        return Result.Ok<IReadOnlyList<UserNotificationDto>>(notifications);
    }
}
