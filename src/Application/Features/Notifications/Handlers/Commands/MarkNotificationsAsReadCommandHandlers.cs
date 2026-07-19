using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Notifications.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Notification;

namespace Tawtheef.Application.Features.Notifications.Handlers.Commands;

public sealed class MarkNotificationAsReadCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<MarkNotificationAsReadCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
    {
        var repo = unitOfWork.GetEntityRepository<Notification>();
        var notification = await repo.DbSet.FirstOrDefaultAsync(
            n => n.Id == request.NotificationId && n.UserId == request.UserId &&
                 n.Channel == NotificationChannel.InApp && !n.IsDismissed,
            cancellationToken);

        if (notification is null)
            return Result.Fail<Unit>(ErrorsCodes.ItemNotFound);

        notification.MarkAsRead();
        await repo.UpdateAsync(notification, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(Unit.Value);
    }
}

public sealed class MarkManyNotificationsAsReadCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<MarkManyNotificationsAsReadCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(MarkManyNotificationsAsReadCommand request, CancellationToken cancellationToken)
    {
        var repo = unitOfWork.GetEntityRepository<Notification>();
        var notifications = await repo.DbSet
            .Where(n => n.UserId == request.UserId && n.Channel == NotificationChannel.InApp &&
                        !n.IsDismissed && !n.IsRead && request.NotificationIds.Contains(n.Id))
            .ToListAsync(cancellationToken);

        foreach (var notification in notifications)
            notification.MarkAsRead();

        if (notifications.Count > 0)
        {
            await repo.UpdateRangeAsync(notifications, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result.Ok(Unit.Value);
    }
}

public sealed class MarkAllNotificationsAsReadCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<MarkAllNotificationsAsReadCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(MarkAllNotificationsAsReadCommand request, CancellationToken cancellationToken)
    {
        var repo = unitOfWork.GetEntityRepository<Notification>();
        var notifications = await repo.DbSet
            .Where(n => n.UserId == request.UserId && n.Channel == NotificationChannel.InApp &&
                        !n.IsDismissed && !n.IsRead)
            .ToListAsync(cancellationToken);

        foreach (var notification in notifications)
            notification.MarkAsRead();

        if (notifications.Count > 0)
        {
            await repo.UpdateRangeAsync(notifications, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result.Ok(Unit.Value);
    }
}
