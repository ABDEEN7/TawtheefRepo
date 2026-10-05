using System.Text.Json;
using Application.Operation.Features.Employee.TestSlots.Commands;
using Application.Operation.Features.Employee.TestSlots.DTOs;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Notification;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Notifications.Templates.TestSessionCandidateScheduledNotification;

namespace Application.Operation.Features.Employee.TestSlots.Handlers.Commands;

public sealed class SendTestSlotCandidateNotificationsCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<SendTestSlotCandidateNotificationsCommand, IResult<SendTestSlotCandidateNotificationsResultDto>>
{
    private static readonly Guid[] AllowedSessionStatuses =
        [TestSessionStatusIds.Approved, TestSessionStatusIds.Ready];

    public async Task<IResult<SendTestSlotCandidateNotificationsResultDto>> Handle(
        SendTestSlotCandidateNotificationsCommand request,
        CancellationToken ct)
    {
        var context = unitOfWork.Context;
        var slotExists = await context.Set<TestSlot>().AsNoTracking()
            .AnyAsync(slot => slot.Id == request.TestSlotId, ct);
        if (!slotExists)
            return Result.Fail<SendTestSlotCandidateNotificationsResultDto>(ErrorsCodes.ItemNotFound);

        if (request.TestSessionId is Guid testSessionId)
        {
            var session = await context.Set<TestSession>().AsNoTracking()
                .Where(item => item.Id == testSessionId)
                .Select(item => new { item.TestSlotId, item.StatusId })
                .FirstOrDefaultAsync(ct);

            if (session is null || session.TestSlotId != request.TestSlotId)
                return Result.Fail<SendTestSlotCandidateNotificationsResultDto>(ErrorsCodes.ItemNotFound);
            if (!AllowedSessionStatuses.Contains(session.StatusId))
                return Result.Fail<SendTestSlotCandidateNotificationsResultDto>(ErrorsCodes.InvalidRequest);
        }

        var candidates = await context.Set<TestSessionCandidate>().AsNoTracking()
            .Where(candidate => candidate.TestSession!.TestSlotId == request.TestSlotId &&
                                AllowedSessionStatuses.Contains(candidate.TestSession.StatusId))
            .Where(candidate => !request.TestSessionId.HasValue ||
                                candidate.TestSessionId == request.TestSessionId.Value)
            .Select(candidate => new
            {
                candidate.TestSessionId,
                UserId = candidate.Invitation!.ApplicantId,
                Email = candidate.Invitation.Applicant!.Email,
                Mobile = candidate.Invitation.Applicant.PhoneNumber,
                Language = candidate.Invitation.Applicant.PreferredLanguage ?? "ar",
                JobNameAr = candidate.TestSession!.Exam!.Job!.JobTitle!.JobNameAr,
                JobNameEn = candidate.TestSession.Exam!.Job!.JobTitle!.JobNameEn ??
                            candidate.TestSession.Exam.Job.JobTitle.JobNameAr,
                TestDate = candidate.TestSession.TestSlot!.SlotDate,
                StartTime = candidate.TestSession.StartTime,
                EndTime = candidate.TestSession.EndTime,
                LocationNameAr = candidate.TestSession.TestSlot.Room!.Location!.NameAr,
                LocationNameEn = candidate.TestSession.TestSlot.Room.Location!.NameEn ??
                                 candidate.TestSession.TestSlot.Room.Location.NameAr,
                RoomNameAr = candidate.TestSession.TestSlot.Room!.NameAr,
                RoomNameEn = candidate.TestSession.TestSlot.Room.NameEn ?? candidate.TestSession.TestSlot.Room.NameAr,
                LocationUrl = candidate.TestSession.TestSlot.Room!.Location!.LocationLink
            })
            .ToListAsync(ct);

        var targets = candidates
            .GroupBy(candidate => new { candidate.TestSessionId, candidate.UserId })
            .Select(group => group.First())
            .ToList();
        if (targets.Count == 0)
            return Result.Fail<SendTestSlotCandidateNotificationsResultDto>(ErrorsCodes.InvalidRequest);
        if (targets.Any(target => target.StartTime is null || target.EndTime is null))
            return Result.Fail<SendTestSlotCandidateNotificationsResultDto>(ErrorsCodes.InvalidRequest);

        var notifications = new List<Notification>(targets.Count * 3);
        foreach (var candidate in targets)
        {
            var model = new TestSessionCandidateScheduledNotificationModel(
                candidate.JobNameAr,
                candidate.JobNameEn,
                candidate.TestDate,
                candidate.StartTime!.Value,
                candidate.EndTime!.Value,
                candidate.LocationNameAr,
                candidate.LocationNameEn,
                candidate.RoomNameAr,
                candidate.RoomNameEn,
                candidate.LocationUrl);
            var payload = JsonSerializer.Serialize(model);
            var language = candidate.Language;
            var templateKey = TestSessionCandidateScheduledNotification.TemplateKey;

            notifications.Add(Notification.Create(
                NotificationChannel.InApp,
                templateKey,
                candidate.UserId,
                candidate.Email,
                payloadJson: payload,
                language: language));

            if (!string.IsNullOrWhiteSpace(candidate.Mobile))
                notifications.Add(Notification.Create(
                    NotificationChannel.Sms,
                    templateKey,
                    candidate.UserId,
                    candidate.Mobile,
                    payloadJson: payload,
                    language: language));

            if (!string.IsNullOrWhiteSpace(candidate.Email))
                notifications.Add(Notification.Create(
                    NotificationChannel.Email,
                    templateKey,
                    candidate.UserId,
                    candidate.Email,
                    payloadJson: payload,
                    language: language));
        }

        await unitOfWork.GetEntityRepository<Notification>().AddRangeAsync(notifications, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Ok(new SendTestSlotCandidateNotificationsResultDto(targets.Count));
    }
}
