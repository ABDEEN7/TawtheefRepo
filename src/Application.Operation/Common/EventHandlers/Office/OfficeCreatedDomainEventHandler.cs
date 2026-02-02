using System.Text.Json;
using Cortex.Mediator.Notifications;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Logging;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Notification;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Domain.Events.Operation.Employee.Office;
using Tawtheef.Notifications.Templates.OfficeCreatedNotification;

namespace Application.Operation.Common.EventHandlers.Office;

public sealed class OfficeCreatedDomainEventHandler(IUnitOfWork unitOfWork, UserManager<User> userManager, IAppLogger logger)
    : INotificationHandler<OfficeCreatedDomainEvent>
{
    public async Task Handle(OfficeCreatedDomainEvent notification, CancellationToken ct)
    {
        var office = await unitOfWork.GetEntityRepository<Tawtheef.Domain.Entities.Lookups.NoneSeeds.Office>().DbSet
            .FirstOrDefaultAsync(o => o.Id == notification.OfficeId, ct);
        var admin = await userManager.Users.FirstOrDefaultAsync(u => u.Id == notification.AdminId, ct);
        if (office is null || admin is null) {
            logger.Error("OfficeCreatedDomainEventHandler: Unable to find office or admin user for OfficeId: {OfficeId}, AdminId: {AdminId}", notification.OfficeId, notification.AdminId);
            return;
        }
        
        var repo = unitOfWork.GetEntityRepository<Notification>();
        var payload = JsonSerializer.Serialize(new OfficeCreatedNotificationModel(office.NameEn, office.Code, admin.Email!));
        var emailNotification = Notification.Create(NotificationChannel.Email, OfficeCreatedNotification.TemplateKey, 
            admin.Id, admin.Email, "Tawtheef Job Invitation", null, payload);
        await repo.AddAsync(emailNotification, ct);
    }
}
