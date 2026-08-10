using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Notifications.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Notification;

namespace Tawtheef.Application.Features.Notifications.Handlers.Commands;

public sealed class DismissNotificationCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<DismissNotificationCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(DismissNotificationCommand request, CancellationToken cancellationToken)
    {
        var repo = unitOfWork.GetEntityRepository<Notification>();
        var notification = await repo.DbSet.FirstOrDefaultAsync(
            n => n.Id == request.NotificationId && n.UserId == request.UserId &&
                 n.Channel == NotificationChannel.InApp && !n.IsDismissed,
            cancellationToken);

        if (notification is null)
            return Result.Fail<Unit>(ErrorsCodes.ItemNotFound);

        notification.Dismiss();
        await repo.UpdateAsync(notification, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(Unit.Value);
    }
}

public sealed class DismissManyNotificationsCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<DismissManyNotificationsCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(DismissManyNotificationsCommand request, CancellationToken cancellationToken)
    {
        var repo = unitOfWork.GetEntityRepository<Notification>();
        var notifications = await repo.DbSet
            .Where(n => n.UserId == request.UserId && n.Channel == NotificationChannel.InApp &&
                        !n.IsDismissed && request.NotificationIds.Contains(n.Id))
            .ToListAsync(cancellationToken);

        foreach (var notification in notifications)
            notification.Dismiss();

        if (notifications.Count > 0)
        {
            await repo.UpdateRangeAsync(notifications, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result.Ok(Unit.Value);
    }
}

public sealed class DismissAllNotificationsCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<DismissAllNotificationsCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(DismissAllNotificationsCommand request, CancellationToken cancellationToken)
    {
        var repo = unitOfWork.GetEntityRepository<Notification>();
        var notifications = await repo.DbSet
            .Where(n => n.UserId == request.UserId && n.Channel == NotificationChannel.InApp && !n.IsDismissed)
            .ToListAsync(cancellationToken);

        foreach (var notification in notifications)
            notification.Dismiss();

        if (notifications.Count > 0)
        {
            await repo.UpdateRangeAsync(notifications, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result.Ok(Unit.Value);
    }
}
