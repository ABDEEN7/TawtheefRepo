using Application.Operation.Features.Employee.TestSessions.DTOs;
using Application.Operation.Features.Employee.TestSessions.Queries;
using Application.Operation.Features.Employee.TestSessions.Services;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Exams;

namespace Application.Operation.Features.Employee.TestSessions.Handlers.Queries;

public sealed class GetTestSessionCapacityQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetTestSessionCapacityQuery, IResult<TestSessionCapacityDto>>
{
    public async Task<IResult<TestSessionCapacityDto>> Handle(GetTestSessionCapacityQuery request, CancellationToken ct)
    {
        var slot = await unitOfWork.Context.Set<TestSlot>().AsNoTracking().Where(x => x.Id == request.TestSlotId)
            .Select(x => new { x.RoomId, x.SlotDate, Capacity = x.Room!.Capacity }).FirstOrDefaultAsync(ct);
        if (slot is null || request.StartTime < TimeOnly.MinValue || request.EndTime <= request.StartTime)
            return Result.Fail<TestSessionCapacityDto>(ErrorsCodes.InvalidRequest);
        if (!TestSessionCapacityService.IsSessionStartCurrentOrFuture(
                slot.SlotDate, request.StartTime, DateTime.UtcNow))
            return Result.Fail<TestSessionCapacityDto>(ErrorsCodes.TestSessionScheduledTimeExpired);
        var sessions = await unitOfWork.Context.Set<TestSession>().AsNoTracking()
            .Where(x => x.TestSlot!.RoomId == slot.RoomId && x.TestSlot.SlotDate == slot.SlotDate)
            .Where(x => x.StartTime.HasValue && x.EndTime.HasValue)
            .Where(x => !TestSessionCapacityService.NonReservingStatusIds.Contains(x.StatusId))
            .Select(x => new TestSessionCapacityReservation(x.StartTime!.Value, x.EndTime!.Value,
                unitOfWork.Context.Set<TestSessionCandidate>().Count(c => c.TestSessionId == x.Id))).ToListAsync(ct);
        var available = Math.Max(0, slot.Capacity - TestSessionCapacityService.CalculatePeakOccupancy(sessions, request.StartTime, request.EndTime));
        return Result.Ok(new TestSessionCapacityDto(available, available >= request.SelectedCandidateCount));
    }
}
