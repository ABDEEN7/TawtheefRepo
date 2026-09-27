using System.Text.Json;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Security;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Notification;
using Tawtheef.Domain.Events.Operation.Employee.Exams;
using Tawtheef.Notifications.Templates.ExamResultReportApprovedNotification;

namespace Application.Operation.Common.EventHandlers.Exams;

public sealed class ExamResultReportApprovedDomainEventHandler(
    IUserRepository userRepository, IUnitOfWork unitOfWork)
    : INotificationHandler<ExamResultReportApprovedDomainEvent>
{
    public async Task Handle(ExamResultReportApprovedDomainEvent notification, CancellationToken ct)
    {
        var exam = await unitOfWork.Context.Set<Exam>().AsNoTracking()
            .Where(x => x.Id == notification.ExamId)
            .Select(x => new
            {
                x.ExamNo, x.TitleAr, x.TitleEn,
                JobTitleAr = x.Job!.JobTitle!.JobNameAr,
                JobTitleEn = x.Job.JobTitle.JobNameEn
            })
            .SingleAsync(ct);
        var recruitmentUsers = await userRepository.GetUsersByPermissionAsync(
            PermissionKeys.Exams.WorkflowActions, ct);
        var payload = JsonSerializer.Serialize(new ExamResultReportApprovedNotificationModel(
            exam.ExamNo, exam.TitleAr, exam.TitleEn,
            exam.JobTitleAr, exam.JobTitleEn, notification.QualifiedCandidateCount));

        foreach (var recipient in recruitmentUsers)
        {
            await unitOfWork.GetEntityRepository<Notification>().AddAsync(Notification.Create(
                NotificationChannel.InApp,
                ExamResultReportApprovedNotification.TemplateKey,
                recipient.Id,
                recipient.Email,
                payloadJson: payload,
                language: recipient.GetPreferredLanguage(),
                idempotencyKey: $"exam-report-approved-{notification.ReportId:N}-{recipient.Id:N}"), ct);
        }
        await unitOfWork.SaveChangesAsync(ct);
    }
}
