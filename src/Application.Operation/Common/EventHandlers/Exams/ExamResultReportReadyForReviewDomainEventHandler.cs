using System.Text.Json;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Security;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Notification;
using Tawtheef.Domain.Events.Operation.Employee.Exams;
using Tawtheef.Notifications.Templates.ExamResultReportReadyNotification;

namespace Application.Operation.Common.EventHandlers.Exams;

public sealed class ExamResultReportReadyForReviewDomainEventHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork) : INotificationHandler<ExamResultReportReadyForReviewDomainEvent>
{
    public async Task Handle(ExamResultReportReadyForReviewDomainEvent notification, CancellationToken ct)
    {
        var exam = await unitOfWork.Context.Set<Exam>()
            .AsNoTracking()
            .Where(x => x.Id == notification.ExamId)
            .Select(x => new
            {
                x.ExamNo,
                x.TitleAr,
                x.TitleEn,
                JobTitleAr = x.Job!.JobTitle!.JobNameAr,
                JobTitleEn = x.Job.JobTitle.JobNameEn
            })
            .SingleAsync(ct);

        var reviewers = await userRepository.GetUsersByPermissionAsync(PermissionKeys.Exams.ApproveResults, ct);

        if (reviewers.Count == 0)
        {
            return;
        }

        var payload = JsonSerializer.Serialize(new ExamResultReportReadyNotificationModel(exam.ExamNo, exam.TitleAr,
            exam.TitleEn, exam.JobTitleAr, exam.JobTitleEn, notification.CandidateCount));

        foreach (var reviewer in reviewers)
        {
            await unitOfWork.GetEntityRepository<Notification>()
                .AddAsync(
                    Notification.Create(NotificationChannel.InApp, ExamResultReportReadyNotification.TemplateKey,
                        reviewer.Id, reviewer.Email, payloadJson: payload, language: reviewer.GetPreferredLanguage(),
                        idempotencyKey: $"exam-report-ready-" + $"{notification.ReportId:N}-"
                                                              + $"{notification.TransitionAtUtc.Ticks:x}-"
                                                              + $"{reviewer.Id:N}"), ct);
        }
    }
}
