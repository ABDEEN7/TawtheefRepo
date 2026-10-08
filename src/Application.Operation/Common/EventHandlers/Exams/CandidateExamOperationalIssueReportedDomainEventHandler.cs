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
using Tawtheef.Notifications.Templates.CandidateExamOperationalIssueReportedNotification;

namespace Application.Operation.Common.EventHandlers.Exams;

public sealed class CandidateExamOperationalIssueReportedDomainEventHandler(
    IUserRepository userRepository, IUnitOfWork unitOfWork)
    : INotificationHandler<CandidateExamOperationalIssueReportedDomainEvent>
{
    public async Task Handle(CandidateExamOperationalIssueReportedDomainEvent notification, CancellationToken ct)
    {
        var reviewers = await userRepository.GetUsersByPermissionAsync(PermissionKeys.Exams.ApproveResults, ct);
        if (reviewers.Count == 0) return;

        var details = await unitOfWork.Context.Set<TestAttempt>().AsNoTracking()
            .Where(attempt => attempt.Id == notification.TestAttemptId
                              && attempt.TestSessionCandidate!.TestSession!.TestSlotId == notification.TestSlotId)
            .Select(attempt => new
            {
                ExamNo = attempt.TestSessionCandidate!.TestSession!.Exam!.ExamNo,
                ExamTitleAr = attempt.TestSessionCandidate.TestSession.Exam.TitleAr,
                ExamTitleEn = attempt.TestSessionCandidate.TestSession.Exam.TitleEn,
                attempt.TestSessionCandidate.TestSession.SessionNo
            })
            .SingleAsync(ct);

        var payload = JsonSerializer.Serialize(new CandidateExamOperationalIssueReportedNotificationModel(
            details.ExamNo, details.ExamTitleAr, details.ExamTitleEn, details.SessionNo,
            notification.TestAttemptId, notification.InterruptionId, notification.DateOccurred));

        foreach (var reviewer in reviewers)
            await unitOfWork.GetEntityRepository<Notification>().AddRangeAsync(
                ExamExecutionNotificationChannels.Create(
                    CandidateExamOperationalIssueReportedNotification.TemplateKey,
                    reviewer, payload, "exam-issue", notification.InterruptionId,
                    candidateCritical: false).ToList(), ct);
    }
}
