using MediatR;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Notifications.DTOs;
using Tawtheef.Application.Features.Notifications.Queries;
using Tawtheef.Domain.Entities.Notification;
using Tawtheef.Notifications.Interfaces;

namespace Tawtheef.Application.Features.Notifications.Handlers.Queries;

public sealed class GetUserNotificationsQueryHandler(IUnitOfWork unitOfWork, 
    UserManager<Domain.Entities.Users.User> userManager,
    IEmailTemplateRenderer renderer)
    : IRequestHandler<GetUserNotificationsQuery, IResult<IReadOnlyList<UserNotificationDto>>>
{
    private const int DefaultLimit = 3;
    private const int MaxLimit = 10;

    public async Task<IResult<IReadOnlyList<UserNotificationDto>>> Handle(
        GetUserNotificationsQuery request,
        CancellationToken cancellationToken)
    {
        var safeLimit = Math.Clamp(request.Limit <= 0 ? DefaultLimit : request.Limit, 1, MaxLimit);
        var user = await userManager.Users.AsNoTracking()
            .SingleAsync(x => x.Id == request.UserId, cancellationToken);

        var notifications = await unitOfWork
            .GetEntityRepository<Notification>()
            .DbSet
            .AsNoTracking()
            .Where(n => n.UserId == request.UserId && n.Channel == NotificationChannel.InApp && !n.IsDismissed)
            .Where(n => !request.UnreadOnly || !n.IsRead)
            .Where(n => !request.CreatedDateBefore.HasValue || n.CreatedDate < request.CreatedDateBefore.Value)
            .OrderByDescending(n => n.CreatedDate)
            .Take(safeLimit)
            .Select(n => new UserNotificationDto
            {
                Id = n.Id,
                Subject = n.Subject,
                Body = n.Body,
                Status = n.Status.ToString(),
                CreatedDate = n.CreatedDate,
                SentAtUtc = n.SentAt,
                Error = n.Error,
                IsRead = n.IsRead,
                TemplateKey = n.TemplateKey,
                PayloadJson = n.PayloadJson
            })
            .ToListAsync(cancellationToken);

        // Render bodies in parallel for notifications that only have payload
        var renderTasks = notifications
            .Where(n => string.IsNullOrWhiteSpace(n.Body) && !string.IsNullOrWhiteSpace(n.PayloadJson))
            .Select(async n =>
            {
                try
                {
                    n.Body = await renderer.RenderHtmlAsync(n.TemplateKey!, n.PayloadJson!, user.PreferredLanguage ?? "ar");
                }
                catch (Exception)
                {
                    // Fallback or leave as null if rendering fails
                }
            });

        await Task.WhenAll(renderTasks);

        // Optional: clear template/payload metadata to keep the API response clean
        foreach (var n in notifications)
        {
            n.TemplateKey = null;
            n.PayloadJson = null;
        }

        return Result.Ok<IReadOnlyList<UserNotificationDto>>(notifications);
    }
}

