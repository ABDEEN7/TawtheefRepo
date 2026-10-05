using Application.Operation.Features.Employee.TestSessions.DTOs;
using Application.Operation.Features.Employee.TestSessions.Queries;
using Application.Operation.Features.Employee.TestSessions.Services;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Employee.TestSessions.Handlers.Queries;

public sealed class GetReadyTestSlotsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetReadyTestSlotsQuery, IResult<ReadyTestSlotsDto>>
{
    public async Task<IResult<ReadyTestSlotsDto>> Handle(GetReadyTestSlotsQuery request, CancellationToken ct)
    {
        var isArabic = string.Equals(request.Language, "ar", StringComparison.OrdinalIgnoreCase);
        var examDuration = await unitOfWork.Context.Set<ExamPart>().AsNoTracking()
            .Where(part => part.ExamId == request.ExamId)
            .SumAsync(part => (int?)part.DurationMinutes, ct) ?? 0;

        if (examDuration == 0) return Result.Fail<ReadyTestSlotsDto>(ErrorsCodes.InvalidRequest);

        var currentDateTime = DateTime.UtcNow;
        var slots = await unitOfWork.Context.Set<TestSlot>().AsNoTracking()
            .Where(slot => slot.StatusId == TestSlotStatusIds.Ready && slot.Room!.StatusId == RoomStatusIds.Active)
            .Where(slot => EF.Functions.DateDiffSecond(slot.StartTime, slot.EndTime)
                           >= examDuration * 60)
            .WhereIf(!string.IsNullOrWhiteSpace(request.SearchText), slot =>
                EF.Functions.Like(slot.TitleAr, $"%{request.SearchText!.Trim()}%") ||
                EF.Functions.Like(slot.TitleEn!, $"%{request.SearchText.Trim()}%"))
            .WhereIf(request.RoomId.HasValue, slot => slot.RoomId == request.RoomId)
            .WhereIf(request.Date.HasValue, slot => slot.SlotDate == request.Date)
            .Select(slot => new SlotRow(slot.Id, isArabic ? slot.TitleAr : slot.TitleEn ?? slot.TitleAr,
                slot.SlotDate, slot.StartTime, slot.EndTime, slot.RoomId,
                isArabic ? slot.Room!.NameAr : slot.Room!.NameEn ?? slot.Room!.NameAr, slot.Room!.Capacity,
                isArabic ? slot.Status!.NameAr : slot.Status!.NameEn))
            .ToListAsync(ct);

        slots = slots.Where(slot => TestSessionCapacityService.HasFutureUsableWindow(
            slot.Date, slot.StartTime, slot.EndTime, examDuration, currentDateTime)).ToList();

        var roomIds = slots.Select(slot => slot.RoomId).Distinct().ToList();
        var dates = slots.Select(slot => slot.Date).Distinct().ToList();
        var sessions = roomIds.Count == 0
            ? []
            : await unitOfWork.Context.Set<TestSession>().AsNoTracking()
                .Where(session => roomIds.Contains(session.TestSlot!.RoomId) && dates.Contains(session.TestSlot.SlotDate))
                .Where(session => session.StartTime.HasValue && session.EndTime.HasValue)
                .Where(session => !TestSessionCapacityService.NonReservingStatusIds.Contains(session.StatusId))
                .Select(session => new SessionRow(session.ExamId, session.TestSlot!.RoomId,
                    session.TestSlot.SlotDate, session.StartTime!.Value, session.EndTime!.Value,
                    unitOfWork.Context.Set<TestSessionCandidate>().Count(candidate => candidate.TestSessionId == session.Id)))
                .ToListAsync(ct);

        var suitableSlots = slots.Select(slot =>
        {
            var slotSessions = sessions.Where(session => session.RoomId == slot.RoomId && session.Date == slot.Date).ToList();
            var reservedSeats = TestSessionCapacityService.CalculatePeakOccupancy(
                slotSessions.Select(session => new TestSessionCapacityReservation(
                    session.Start, session.End, session.CandidateCount)), slot.StartTime, slot.EndTime);
            var availableSeats = Math.Max(0, slot.RoomCapacity - reservedSeats);
            return new ReadyTestSlotListItemDto
            {
                TestSlotId = slot.Id, SlotName = slot.Name, SlotDate = slot.Date, StartTime = slot.StartTime,
                EndTime = slot.EndTime, RoomId = slot.RoomId, RoomName = slot.RoomName,
                RoomCapacity = slot.RoomCapacity, CurrentReservations = reservedSeats,
                ExistingSessionCount = slotSessions.Count, ExistingExamSessionCount = slotSessions.Count(s => s.ExamId == request.ExamId),
                RemainingCapacity = availableSeats, Status = slot.Status
            };
        }).OrderBy(slot => slot.SlotDate).ThenBy(slot => slot.StartTime).ToList();

        var pageNumber = Math.Max(1, request.PageNumber);
        var pageSize = Math.Clamp(request.PageSize, 1, 50);
        var page = suitableSlots.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        return Result.Ok(new ReadyTestSlotsDto(new PaginatedResult<ReadyTestSlotListItemDto>(page,
            suitableSlots.Count, pageNumber, pageSize), new ReadyTestSlotsSummaryDto(0, 0, 0)));
    }

    private sealed record SlotRow(Guid Id, string Name, DateOnly Date, TimeOnly StartTime, TimeOnly EndTime,
        Guid RoomId, string RoomName, int RoomCapacity, string Status);
    private sealed record SessionRow(Guid ExamId, Guid RoomId, DateOnly Date, TimeOnly Start, TimeOnly End, int CandidateCount);
}
