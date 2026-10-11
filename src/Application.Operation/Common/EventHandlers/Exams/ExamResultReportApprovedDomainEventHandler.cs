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
using Tawtheef.Notifications.Templates.ExamResultReportApprovedNotification;

namespace Application.Operation.Common.EventHandlers.Exams;

public sealed class ExamResultReportApprovedDomainEventHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork)
    : INotificationHandler<
        ExamResultReportApprovedDomainEvent>
{
    public async Task Handle(
        ExamResultReportApprovedDomainEvent notification,
        CancellationToken ct)
    {
        var exam = await unitOfWork.Context
            .Set<Exam>()
            .AsNoTracking()
            .Where(x => x.Id == notification.ExamId)
            .Select(x => new
            {
                x.ExamNo,
                x.TitleAr,
                x.TitleEn,
                JobTitleAr =
                    x.Job!.JobTitle!.JobNameAr,
                JobTitleEn =
                    x.Job.JobTitle.JobNameEn
            })
            .SingleAsync(ct);

        var interviewSchedulingUsers =
            await userRepository
                .GetUsersByPermissionAsync(
                    PermissionKeys
                        .InterviewSchedule
                        .Manage,
                    ct);

        if (interviewSchedulingUsers.Count == 0)
        {
            return;
        }

        var payload =
            JsonSerializer.Serialize(
                new ExamResultReportApprovedNotificationModel(
                    exam.ExamNo,
                    exam.TitleAr,
                    exam.TitleEn,
                    exam.JobTitleAr,
                    exam.JobTitleEn,
                    notification
                        .QualifiedCandidateCount));

        foreach (
            var recipient in
            interviewSchedulingUsers)
        {
            await unitOfWork.GetEntityRepository<Notification>().AddRangeAsync(
                ExamExecutionNotificationChannels.Create(ExamResultReportApprovedNotification.TemplateKey,
                    recipient, payload, "rpt-appr", notification.ReportId, candidateCritical: false).ToList(), ct);
        }

        await unitOfWork.SaveChangesAsync(ct);
    }
}
