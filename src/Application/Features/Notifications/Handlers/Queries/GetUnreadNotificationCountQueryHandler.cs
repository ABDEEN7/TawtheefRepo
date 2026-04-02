using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Notifications.Queries;
using Tawtheef.Domain.Entities.Notification;

namespace Tawtheef.Application.Features.Notifications.Handlers.Queries;

public sealed class GetUnreadNotificationCountQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetUnreadNotificationCountQuery, IResult<int>>
{
    public async Task<IResult<int>> Handle(
        GetUnreadNotificationCountQuery request,
        CancellationToken cancellationToken)
    {
        var count = await unitOfWork
            .GetEntityRepository<Notification>()
            .DbSet
            .AsNoTracking()
            .CountAsync(n => n.UserId == request.UserId 
                          && n.Channel == NotificationChannel.InApp 
                          && !n.IsDismissed 
                          && !n.IsRead, cancellationToken);

        return Result.Ok(count);
    }
}
