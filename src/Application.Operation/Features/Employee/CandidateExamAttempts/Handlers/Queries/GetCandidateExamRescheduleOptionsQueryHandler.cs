using Application.Operation.Features.Employee.CandidateExamAttempts.DTOs;
using Application.Operation.Features.Employee.CandidateExamAttempts.Queries;
using Application.Operation.Features.Employee.TestSessions.Services;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Employee.CandidateExamAttempts.Handlers.Queries;

public sealed class GetCandidateExamRescheduleOptionsQueryHandler(IUnitOfWork unitOfWork, TimeProvider timeProvider)
    : IRequestHandler<GetCandidateExamRescheduleOptionsQuery, IResult<IReadOnlyList<CandidateExamRescheduleOptionDto>>>
{
    public async Task<IResult<IReadOnlyList<CandidateExamRescheduleOptionDto>>> Handle(
        GetCandidateExamRescheduleOptionsQuery request, CancellationToken cancellationToken)
    {
        var state = await unitOfWork.Context.Set<TestAttempt>()
            .AsNoTracking()
            .Where(x => x.Id == request.TestAttemptId)
            .Select(x => new
            {
                AttemptStatusId = x.StatusId,
                x.ActiveSessionId,
                AssignmentId = x.TestSessionCandidateId,
                AssignmentStatusId = x.TestSessionCandidate!.StatusId,
                x.TestSessionCandidate.InvitationId,
                CurrentSessionId = x.TestSessionCandidate.TestSessionId,
                x.TestSessionCandidate.TestSession!.ExamId,
                x.TestSessionCandidate.TestSession.TestSlotId,
                SessionStatusId = x.TestSessionCandidate.TestSession.StatusId,
                SlotStatusId = x.TestSessionCandidate.TestSession.TestSlot!.StatusId,
                x.TestSessionCandidate.TestSession.Exam!.InterruptionPolicyId
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (state is null)
            return Unavailable();

        if (state.AttemptStatusId != TestAttemptStatusIds.Interrupted || state.ActiveSessionId is not null
                                                                      || state.AssignmentStatusId
                                                                      != TestSessionCandidateStatusIds.Started
                                                                      || state.SessionStatusId
                                                                      != TestSessionStatusIds.InProgress
                                                                      || state.SlotStatusId != TestSlotStatusIds.Started
                                                                      || (state.InterruptionPolicyId
                                                                          != ExamInterruptionPolicyIds.RescheduleOnly
                                                                          && state.InterruptionPolicyId
                                                                          != ExamInterruptionPolicyIds
                                                                              .ResumeOrReschedule))
        {
            return Unavailable();
        }

        var isHallSupervisor = await unitOfWork.Context.Set<TestSlotStaff>()
            .AsNoTracking()
            .AnyAsync(
                x => x.TestSlotId == state.TestSlotId && x.StaffUserId == request.OfficerId && x.IsActive
                     && x.RoleId == TestSlotStaffRoleIds.HallSupervisor, cancellationToken);

        if (!isHallSupervisor)
        {
            return Result.Fail<IReadOnlyList<CandidateExamRescheduleOptionDto>>(
                new Error(ErrorsCodes.UnauthorizedAction).WithMetadata("StatusCode", StatusCodes.Status403Forbidden));
        }

        var interruptions = await unitOfWork.Context.Set<TestAttemptInterruption>()
            .AsNoTracking()
            .Where(x => x.TestAttemptId == request.TestAttemptId && x.StatusId == TestAttemptInterruptionStatusIds.Open)
            .Select(x => new { x.TestAttemptPartId, x.ResolutionActionId, x.ResolvedById, x.ResolvedAt })
            .Take(2)
            .ToListAsync(cancellationToken);

        if (interruptions.Count != 1 || interruptions[0].TestAttemptPartId is not { } interruptedPartId
                                     || interruptions[0].ResolutionActionId is not null
                                     || interruptions[0].ResolvedById is not null
                                     || interruptions[0].ResolvedAt is not null)
        {
            return Unavailable();
        }

        var interruptedPartExists = await unitOfWork.Context.Set<TestAttemptPart>()
            .AsNoTracking()
            .AnyAsync(
                x => x.Id == interruptedPartId && x.TestAttemptId == request.TestAttemptId
                                               && x.StatusId == TestAttemptPartStatusIds.Interrupted,
                cancellationToken);

        if (!interruptedPartExists)
            return Unavailable();

        var alreadyRescheduled = await unitOfWork.Context.Set<TestSessionCandidate>()
            .AsNoTracking()
            .AnyAsync(x => x.RescheduledFromTestSessionCandidateId == state.AssignmentId, cancellationToken);

        if (alreadyRescheduled)
        {
            return Result.Fail<IReadOnlyList<CandidateExamRescheduleOptionDto>>(ErrorsCodes
                .CandidateExamAlreadyRescheduled);
        }

        var now = timeProvider.GetLocalNow().DateTime;

        var today = DateOnly.FromDateTime(now);
        var currentTime = TimeOnly.FromDateTime(now);

        var rawOptions = await unitOfWork.Context.Set<TestSession>()
            .AsNoTracking()
            .Where(x => x.ExamId == state.ExamId && x.Id != state.CurrentSessionId && x.StartTime.HasValue
                        && x.EndTime.HasValue && x.StartTime < x.EndTime
                        && (x.StatusId == TestSessionStatusIds.Approved || x.StatusId == TestSessionStatusIds.Ready)
                        && x.TestSlot != null
                        && (x.TestSlot.StatusId == TestSlotStatusIds.Ready
                            || x.TestSlot.StatusId == TestSlotStatusIds.Started) && x.TestSlot.Room != null
                        && x.TestSlot.Room.StatusId == RoomStatusIds.Active && x.TestSlot.Room.Capacity > 0
                        && x.StartTime >= x.TestSlot.StartTime && x.EndTime <= x.TestSlot.EndTime
                        && (x.TestSlot.SlotDate > today || (x.TestSlot.SlotDate == today && x.StartTime > currentTime)))
            .Select(x => new
            {
                x.Id,
                x.SessionNo,
                x.StartTime,
                x.EndTime,
                TestSlotId = x.TestSlot!.Id,
                x.TestSlot.TitleAr,
                x.TestSlot.TitleEn,
                x.TestSlot.SlotDate,
                RoomId = x.TestSlot.Room!.Id,
                RoomNameAr = x.TestSlot.Room.NameAr,
                RoomNameEn = x.TestSlot.Room.NameEn,
                x.TestSlot.Room.Capacity,
                AssignedCount = unitOfWork.Context.Set<TestSessionCandidate>()
                    .Count(candidate => candidate.TestSessionId == x.Id
                                        && candidate.StatusId != TestSessionCandidateStatusIds.Cancelled
                                        && candidate.StatusId != TestSessionCandidateStatusIds.Rescheduled)
            })
            .ToListAsync(cancellationToken);

        if (rawOptions.Count == 0)
        {
            return Result.Ok<IReadOnlyList<CandidateExamRescheduleOptionDto>>([]);
        }

        var roomIds = rawOptions.Select(x => x.RoomId).Distinct().ToArray();

        var slotDates = rawOptions.Select(x => x.SlotDate).Distinct().ToArray();

        var reservationRows = await unitOfWork.Context.Set<TestSession>()
            .AsNoTracking()
            .Where(x => x.TestSlot != null && roomIds.Contains(x.TestSlot.RoomId)
                                           && slotDates.Contains(x.TestSlot.SlotDate) && x.StartTime.HasValue
                                           && x.EndTime.HasValue
                                           && !TestSessionCapacityService.NonReservingStatusIds.Contains(x.StatusId))
            .Select(x => new
            {
                x.TestSlot!.RoomId,
                x.TestSlot.SlotDate,
                StartTime = x.StartTime!.Value,
                EndTime = x.EndTime!.Value,
                CandidateCount = unitOfWork.Context.Set<TestSessionCandidate>()
                    .Count(candidate => candidate.TestSessionId == x.Id && candidate.Id != state.AssignmentId
                                                                        && candidate.StatusId
                                                                        != TestSessionCandidateStatusIds.Cancelled
                                                                        && candidate.StatusId
                                                                        != TestSessionCandidateStatusIds
                                                                            .Rescheduled)
            })
            .ToListAsync(cancellationToken);

        var reservationsByRoomDate = reservationRows.GroupBy(x => new { x.RoomId, x.SlotDate })
            .ToDictionary(group => (group.Key.RoomId, group.Key.SlotDate),
                group => group.Select(x => new TestSessionCapacityReservation(x.StartTime, x.EndTime, x.CandidateCount))
                    .ToArray());

        var options = rawOptions.Select(option =>
            {
                reservationsByRoomDate.TryGetValue((option.RoomId, option.SlotDate), out var reservations);

                var occupied = TestSessionCapacityService.CalculatePeakOccupancy(reservations ?? [],
                    option.StartTime!.Value, option.EndTime!.Value);

                var available = Math.Max(0, option.Capacity - occupied);

                return new { Option = option, AvailableCapacity = available };
            })
            .Where(x => x.AvailableCapacity > 0)
            .OrderBy(x => x.Option.SlotDate)
            .ThenBy(x => x.Option.StartTime)
            .ThenBy(x => x.Option.SessionNo)
            .Select(x => new CandidateExamRescheduleOptionDto(x.Option.Id, x.Option.SessionNo, x.Option.TestSlotId,
                x.Option.TitleAr, x.Option.TitleEn, x.Option.SlotDate, x.Option.StartTime!.Value,
                x.Option.EndTime!.Value,
                x.Option.RoomId, x.Option.RoomNameAr, x.Option.RoomNameEn, x.Option.Capacity, x.Option.AssignedCount,
                x.AvailableCapacity))
            .ToList();

        return Result.Ok<IReadOnlyList<CandidateExamRescheduleOptionDto>>(options);
    }

    private static IResult<IReadOnlyList<CandidateExamRescheduleOptionDto>> Unavailable() =>
        Result.Fail<IReadOnlyList<CandidateExamRescheduleOptionDto>>(ErrorsCodes.CandidateExamRescheduleUnavailable);
}
