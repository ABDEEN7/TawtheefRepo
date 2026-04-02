using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Notifications.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Notification;

namespace Tawtheef.Application.Features.Notifications.Handlers.Commands;

public sealed class UpdateNotificationStateCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateNotificationStateCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(UpdateNotificationStateCommand request, CancellationToken cancellationToken)
    {
        var repo = unitOfWork.GetEntityRepository<Notification>();
        
        var notification = await repo.DbSet.FirstOrDefaultAsync(n => n.Id == request.NotificationId && n.UserId == request.UserId, cancellationToken);
        
        if (notification == null)
            return Result.Fail<Unit>(ErrorsCodes.ItemNotFound);

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

        await repo.UpdateAsync(notification, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(Unit.Value);
    }
}
