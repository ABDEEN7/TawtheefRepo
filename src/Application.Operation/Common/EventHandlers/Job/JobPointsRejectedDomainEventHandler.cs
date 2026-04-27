using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Domain.Entities.Notification;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Domain.Events.Operation.Employee.Job;
using Tawtheef.Notifications.Templates.JobPointsRejectedNotification;

namespace Application.Operation.Common.EventHandlers.Job;

public sealed class JobPointsRejectedDomainEventHandler(
    UserManager<User> userManager,
    ILocalizationService localizationService,
    IUnitOfWork unitOfWork)
    : INotificationHandler<JobPointsRejectedDomainEvent>
{
    public async Task Handle(JobPointsRejectedDomainEvent notification, CancellationToken ct)
    {
        var jobTitle = localizationService.GetLocalizedValue(notification.Job.JobTitle?.JobNameAr ?? string.Empty, notification.Job.JobTitle?.JobNameEn ?? string.Empty);
        var payload = JsonSerializer.Serialize(new JobPointsRejectedNotificationModel(jobTitle, notification.Reason));

        var jobPointsMain = await unitOfWork.GetEntityRepository<JobPointsMain>()
            .DbSet.AsNoTracking()
            .FirstOrDefaultAsync(x => x.JobId == notification.Job.Id, ct);

        if (jobPointsMain?.CreatedById == null)
            return;

        var user = await userManager.FindByIdAsync(jobPointsMain.CreatedById.ToString()!);
        if (user == null || string.IsNullOrWhiteSpace(user.Email))
            return;

        var repo = unitOfWork.GetEntityRepository<Notification>();
        var notificationEntity = Notification.Create(NotificationChannel.InApp, JobPointsRejectedNotification.TemplateKey, user.Id, 
            user.Email, null, null, null, payload, user.GetPreferredLanguage(), null, 3);
        await repo.AddAsync(notificationEntity, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
