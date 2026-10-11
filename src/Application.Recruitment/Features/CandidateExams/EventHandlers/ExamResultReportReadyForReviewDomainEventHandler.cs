using System.Text.Json;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Notifications;
using Tawtheef.Application.Common.Security;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Notification;
using Tawtheef.Domain.Events.Operation.Employee.Exams;
using Tawtheef.Notifications.Templates.ExamResultReportReadyNotification;

namespace Application.Recruitment.Features.CandidateExams.EventHandlers;

public sealed class ExamResultReportReadyForReviewDomainEventHandler(
    IUserRepository userRepository, IUnitOfWork unitOfWork)
    : INotificationHandler<ExamResultReportReadyForReviewDomainEvent>
{
    public async Task Handle(ExamResultReportReadyForReviewDomainEvent notification, CancellationToken ct)
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
        var reviewers = await userRepository.GetUsersByPermissionAsync(
            PermissionKeys.Exams.ApproveResults, ct);
        var payload = JsonSerializer.Serialize(new ExamResultReportReadyNotificationModel(
            exam.ExamNo, exam.TitleAr, exam.TitleEn,
            exam.JobTitleAr, exam.JobTitleEn, notification.CandidateCount));

        foreach (var reviewer in reviewers)
        {
            await unitOfWork.GetEntityRepository<Notification>().AddRangeAsync(
                ExamExecutionNotificationChannels.Create(ExamResultReportReadyNotification.TemplateKey,
                    reviewer, payload, $"rpt-ready-{notification.TransitionAtUtc.Ticks:x}",
                    notification.ReportId, candidateCritical: false).ToList(), ct);
        }
    }
}
