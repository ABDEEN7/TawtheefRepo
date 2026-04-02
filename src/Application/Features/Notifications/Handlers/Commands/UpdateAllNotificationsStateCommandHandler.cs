using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Notifications.Commands;
using Tawtheef.Domain.Entities.Notification;

namespace Tawtheef.Application.Features.Notifications.Handlers.Commands;

public sealed class UpdateAllNotificationsStateCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateAllNotificationsStateCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(UpdateAllNotificationsStateCommand request, CancellationToken cancellationToken)
    {
        var repo = unitOfWork.GetEntityRepository<Notification>();
        
        var query = repo.DbSet.Where(n => n.UserId == request.UserId && n.Channel == NotificationChannel.InApp && !n.IsDismissed);

        if (request.Action == NotificationAction.MarkAsRead)
        {
            query = query.Where(n => !n.IsRead);
        }
        else if (request.Action == NotificationAction.MarkAsUnread)
        {
            query = query.Where(n => n.IsRead);
        }

        var notifications = await query.ToListAsync(cancellationToken);

        if (!notifications.Any())
            return Result.Ok(Unit.Value);

        foreach (var notification in notifications)
        {
            switch (request.Action)
            {
                case NotificationAction.MarkAsRead:
                    notification.MarkAsRead();
                    break;
                case NotificationAction.MarkAsUnread:
                    notification.MarkAsUnread();
                    break;
                case NotificationAction.Dismiss:
                    notification.Dismiss();
                    break;
            }
        }

        await repo.UpdateRangeAsync(notifications, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(Unit.Value);
    }
}
