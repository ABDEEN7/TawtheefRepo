using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Logging;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Notification;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Domain.Events.Operation.Employee.Profile;
using Tawtheef.Notifications.Templates.ProfileAssigned;
using Notification = Tawtheef.Domain.Entities.Notification.Notification;

namespace Application.Operation.Common.EventHandlers.Profile;

public class ProfileAssignedEventHandler(
    IUnitOfWork uow,
    UserManager<User> userManager,
    IAppLogger logger)
    : INotificationHandler<ProfileAssignedEvent>
{
    private readonly IAppLogger _log = logger.ForContext(nameof(ProfileAssignedEventHandler));

    public async Task Handle(ProfileAssignedEvent @event, CancellationToken ct)
    {
        var employee = await userManager.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == @event.EmployeeId, ct);

        if (employee == null)
        {
            _log.Error("Employee with id {EmployeeId} not found for ProfileAssignedEvent", @event.EmployeeId);
            return;
        }

        var profile = await uow.GetEntityRepository<UserProfile>().DbSet
            .AsNoTracking()
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == @event.UserProfileId, ct);

        if (profile == null)
        {
            _log.Error("Profile with id {ProfileId} not found for ProfileAssignedEvent", @event.UserProfileId);
            return;
        }

        var payload = JsonSerializer.Serialize(new ProfileAssignedModel(profile.Id, profile.User?.FullNameEn ?? "Candidate"));
        var notificationEmail = Notification.Create(
            NotificationChannel.Email,
            ProfileAssigned.TemplateKey,
            employee.Id,
            employee.Email,
            null,
            null, null,
            payload, employee.GetPreferredLanguage(), null, 3);

        var notificationInApp = Notification.Create(
            NotificationChannel.InApp,
            ProfileAssigned.TemplateKey,
            employee.Id,
            employee.Email,
            null,
            null, null,
            payload, employee.GetPreferredLanguage(), null, 3);

        await uow.GetEntityRepository<Notification>().AddAsync(notificationEmail, ct);
        await uow.GetEntityRepository<Notification>().AddAsync(notificationInApp, ct);
        
        await uow.SaveChangesAsync(ct);
    }
}
