using Application.Operation.Features.Employee.TestSessions.DTOs;
using Application.Operation.Features.Employee.TestSessions.Queries;
using Application.Operation.Features.Employee.TestSessions.Services;
using System.Text.Json;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Logger;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.TestSessions.Handlers.Queries;

public sealed class GetTestSessionForEditQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetTestSessionForEditQuery, IResult<TestSessionEditDto>>
{
    public async Task<IResult<TestSessionEditDto>> Handle(GetTestSessionForEditQuery request, CancellationToken ct)
    {
        var isArabic = string.Equals(request.Language, "ar", StringComparison.OrdinalIgnoreCase);
        var session = await unitOfWork.Context.Set<TestSession>().AsNoTracking()
            .Where(x => x.Id == request.TestSessionId)
            .Select(x => new
            {
                x.Id,
                x.SessionNo,
                x.ExamId,
                x.StatusId,
                x.GenderFilter,
                x.NationalityFilter,
                x.TestSlotId,
                x.StartTime,
                x.EndTime,
                InvitationIds = x.TestSessionCandidates.Select(candidate => candidate.InvitationId).ToList(),
                SlotName = x.TestSlot == null ? null : isArabic ? x.TestSlot.TitleAr : x.TestSlot.TitleEn ?? x.TestSlot.TitleAr,
                SlotDate = x.TestSlot == null ? null : (DateOnly?)x.TestSlot.SlotDate,
                SlotStartTime = x.TestSlot == null ? null : (TimeOnly?)x.TestSlot.StartTime,
                SlotEndTime = x.TestSlot == null ? null : (TimeOnly?)x.TestSlot.EndTime,
                RoomId = x.TestSlot == null ? null : (Guid?)x.TestSlot.RoomId,
                RoomName = x.TestSlot == null ? null : isArabic
                    ? x.TestSlot.Room!.NameAr
                    : x.TestSlot.Room!.NameEn ?? x.TestSlot.Room.NameAr,
                RoomCapacity = x.TestSlot == null ? null : (int?)x.TestSlot.Room!.Capacity,
                RoomIdForCapacity = x.TestSlot == null ? null : (Guid?)x.TestSlot.RoomId,
                SlotDateForCapacity = x.TestSlot == null ? null : (DateOnly?)x.TestSlot.SlotDate
            }).FirstOrDefaultAsync(ct);

        if (session is null || !request.ViewMode && !IsEditable(session.StatusId))
            return Result.Fail<TestSessionEditDto>(ErrorsCodes.InvalidRequest);

        var availableCapacity = 0;
        if (session.TestSlotId.HasValue && session.StartTime.HasValue && session.EndTime.HasValue &&
            session.RoomIdForCapacity.HasValue && session.SlotDateForCapacity.HasValue && session.RoomCapacity.HasValue)
        {
            var reservations = await unitOfWork.Context.Set<TestSession>().AsNoTracking()
                .Where(existing => existing.Id != session.Id &&
                    existing.TestSlot!.RoomId == session.RoomIdForCapacity &&
                    existing.TestSlot.SlotDate == session.SlotDateForCapacity &&
                    existing.StartTime.HasValue && existing.EndTime.HasValue &&
                    !TestSessionCapacityService.NonReservingStatusIds.Contains(existing.StatusId))
                .Select(existing => new TestSessionCapacityReservation(
                    existing.StartTime!.Value,
                    existing.EndTime!.Value,
                    unitOfWork.Context.Set<TestSessionCandidate>().Count(candidate =>
                        candidate.TestSessionId == existing.Id)))
                .ToListAsync(ct);

            availableCapacity = Math.Max(0, session.RoomCapacity.Value -
                TestSessionCapacityService.CalculatePeakOccupancy(
                    reservations, session.StartTime.Value, session.EndTime.Value));
        }

        string? decisionNote = null;
        string? decisionByName = null;
        DateTime? decisionAt = null;
        var statusBackendName = session.StatusId == TestSessionStatusIds.Rejected
            ? "REJECTED"
            : session.StatusId == TestSessionStatusIds.Returned ? "RETURNED" : null;
        if (session.StatusId == TestSessionStatusIds.Returned ||
            session.StatusId == TestSessionStatusIds.Rejected)
        {
            var actionLog = await unitOfWork.Context.Set<ActionLog>().AsNoTracking()
                .Where(log => log.EntityId == session.Id && log.Section == "TestSessionWorkflow" &&
                              (log.ActionType == "TestSessionReturnedForEdit" ||
                               log.ActionType == "TestSessionRejected"))
                .OrderByDescending(log => log.CreatedDate)
                .Select(log => new { log.UserId, log.Notes, log.CreatedDate })
                .FirstOrDefaultAsync(ct);
            if (actionLog?.UserId is { } reviewerId)
            {
                decisionByName = await unitOfWork.Context.Set<User>().AsNoTracking()
                    .Where(user => user.Id == reviewerId)
                    .Select(user => user.FullNameEn ?? user.FullNameAr)
                    .FirstOrDefaultAsync(ct);
            }
            if (!string.IsNullOrWhiteSpace(actionLog?.Notes))
            {
                using var document = JsonDocument.Parse(actionLog.Notes);
                if (document.RootElement.TryGetProperty("decisionNote", out var noteElement) ||
                    document.RootElement.TryGetProperty("returnNote", out noteElement))
                    decisionNote = noteElement.GetString();
                if (document.RootElement.TryGetProperty("performedAt", out var performedAtElement) &&
                    performedAtElement.TryGetDateTime(out var performedAt))
                    decisionAt = performedAt;
            }
            decisionAt ??= actionLog?.CreatedDate;
        }

        return Result.Ok(new TestSessionEditDto(
            session.Id, session.SessionNo, session.ExamId, session.StatusId,
            session.GenderFilter, session.NationalityFilter, session.InvitationIds,
            session.TestSlotId, session.SlotName, session.SlotDate, session.SlotStartTime,
            session.SlotEndTime, session.RoomId, session.RoomName, session.RoomCapacity,
            session.StartTime, session.EndTime, availableCapacity, decisionNote, decisionByName,
            decisionAt, statusBackendName));
    }

    private static bool IsEditable(Guid statusId) =>
        statusId == TestSessionStatusIds.Draft || statusId == TestSessionStatusIds.Returned;

}
