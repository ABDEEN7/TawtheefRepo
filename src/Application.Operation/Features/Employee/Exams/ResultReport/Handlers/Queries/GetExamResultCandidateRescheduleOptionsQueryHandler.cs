using Application.Operation.Features.Employee.Exams.ResultReport.DTOs;
using Application.Operation.Features.Employee.Exams.ResultReport.Queries;
using Application.Operation.Features.Employee.TestSessions.Services;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Employee.Exams.ResultReport.Handlers.Queries;

public sealed class GetExamResultCandidateRescheduleOptionsQueryHandler(
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IRequestHandler<GetExamResultCandidateRescheduleOptionsQuery,
        IResult<IReadOnlyList<ExamResultCandidateRescheduleOptionDto>>>
{
    public async Task<IResult<IReadOnlyList<ExamResultCandidateRescheduleOptionDto>>> Handle(
        GetExamResultCandidateRescheduleOptionsQuery request,
        CancellationToken cancellationToken)
    {
        var report = await unitOfWork.Context.Set<ExamResultReport>()
            .AsNoTracking()
            .Where(x => x.Id == request.ReportId)
            .Select(x => new { x.ExamId, x.StatusId })
            .SingleOrDefaultAsync(cancellationToken);
        if (report is null)
            return Failure(ErrorsCodes.ExamResultReportNotFound);

        var candidate = await unitOfWork.Context.Set<ExamResultCandidate>()
            .AsNoTracking()
            .Where(x => x.Id == request.CandidateResultId &&
                        x.ExamResultReportId == request.ReportId)
            .Select(x => new { x.InvitationId, x.ResultStatusId, x.TestSessionCandidateId, x.TestAttemptId })
            .SingleOrDefaultAsync(cancellationToken);
        if (candidate is null)
            return Failure(ErrorsCodes.ExamResultCandidateNotFound);
        if (report.StatusId != ExamResultReportStatusIds.Returned)
            return Failure(ErrorsCodes.ExamResultReportNotReturned);
        if (candidate.ResultStatusId != ExamResultCandidateResultStatusIds.RequiresAction)
            return Failure(ErrorsCodes.ExamResultCandidateNotRequiresAction);
        if (candidate.TestSessionCandidateId is not { } currentAssignmentId)
            return Failure(ErrorsCodes.ExamResultCandidateRescheduleStateInvalid);

        var currentAssignment = await unitOfWork.Context.Set<TestSessionCandidate>()
            .AsNoTracking()
            .Where(x => x.Id == currentAssignmentId)
            .Select(x => new
            {
                x.InvitationId,
                x.TestSessionId,
                x.StatusId,
                x.TestSession!.ExamId,
                AttemptId = x.TestAttempt == null ? (Guid?)null : x.TestAttempt.Id
            })
            .SingleOrDefaultAsync(cancellationToken);
        if (currentAssignment is null || currentAssignment.InvitationId != candidate.InvitationId ||
            currentAssignment.ExamId != report.ExamId ||
            currentAssignment.AttemptId != candidate.TestAttemptId ||
            currentAssignment.StatusId != TestSessionCandidateStatusIds.Assigned &&
            currentAssignment.StatusId != TestSessionCandidateStatusIds.Authorized &&
            currentAssignment.StatusId != TestSessionCandidateStatusIds.Started &&
            currentAssignment.StatusId != TestSessionCandidateStatusIds.Completed)
            return Failure(ErrorsCodes.ExamResultCandidateRescheduleStateInvalid);

        if (await unitOfWork.Context.Set<TestSessionCandidate>().AsNoTracking()
                .AnyAsync(x => x.RescheduledFromTestSessionCandidateId == currentAssignmentId,
                    cancellationToken))
            return Failure(ErrorsCodes.ExamResultCandidateAlreadyRescheduled);

        if (candidate.TestAttemptId is { } attemptId)
        {
            var attempt = await unitOfWork.Context.Set<TestAttempt>().AsNoTracking()
                .Where(x => x.Id == attemptId &&
                            x.TestSessionCandidateId == currentAssignmentId)
                .Select(x => new { x.StatusId, x.ActiveSessionId })
                .SingleOrDefaultAsync(cancellationToken);
            if (attempt is null)
                return Failure(ErrorsCodes.ExamResultCandidateRescheduleStateInvalid);

            var interruptions = await unitOfWork.Context.Set<TestAttemptInterruption>()
                .AsNoTracking()
                .Where(x => x.TestAttemptId == attemptId &&
                            x.StatusId == TestAttemptInterruptionStatusIds.Open)
                .Select(x => new { x.TestAttemptPartId, x.ResolvedAt })
                .Take(2)
                .ToListAsync(cancellationToken);
            if (interruptions.Count != 1 || interruptions[0].ResolvedAt is not null)
                return Failure(ErrorsCodes.ExamResultCandidateInterruptionStateInvalid);

            if (attempt.StatusId == TestAttemptStatusIds.Interrupted)
            {
                if (attempt.ActiveSessionId is not null ||
                    interruptions[0].TestAttemptPartId is not { } partId ||
                    !await unitOfWork.Context.Set<TestAttemptPart>().AsNoTracking()
                        .AnyAsync(x => x.Id == partId && x.TestAttemptId == attemptId &&
                                       x.StatusId == TestAttemptPartStatusIds.Interrupted, cancellationToken))
                    return Failure(ErrorsCodes.ExamResultCandidateInterruptionStateInvalid);
            }
            else if (attempt.StatusId != TestAttemptStatusIds.Completed &&
                     attempt.StatusId != TestAttemptStatusIds.TimedOut)
                return Failure(ErrorsCodes.ExamResultCandidateRescheduleStateInvalid);
        }

        if (await unitOfWork.Context.Set<TestSessionCandidate>().AsNoTracking()
                .AnyAsync(x => x.Id != currentAssignmentId &&
                               x.InvitationId == candidate.InvitationId &&
                               x.TestSession!.ExamId == report.ExamId &&
                               x.StatusId != TestSessionCandidateStatusIds.Cancelled &&
                               x.StatusId != TestSessionCandidateStatusIds.Rescheduled, cancellationToken))
            return Failure(ErrorsCodes.ExamResultCandidateActiveAssignmentExists);

        var now =
            timeProvider.GetLocalNow().DateTime;

        var today =
            DateOnly.FromDateTime(now);

        var currentTime =
            TimeOnly.FromDateTime(now);

        var rawOptions = await unitOfWork.Context
            .Set<TestSession>()
            .AsNoTracking()
            .Where(x =>
                x.ExamId == report.ExamId &&
                x.Id != currentAssignment.TestSessionId &&
                x.StartTime.HasValue &&
                x.EndTime.HasValue &&
                x.StartTime < x.EndTime &&
                (
                    x.StatusId ==
                    TestSessionStatusIds.Approved ||
                    x.StatusId ==
                    TestSessionStatusIds.Ready
                ) &&
                x.TestSlot != null &&
                (
                    x.TestSlot.StatusId ==
                    TestSlotStatusIds.Ready ||
                    x.TestSlot.StatusId ==
                    TestSlotStatusIds.Started
                ) &&
                x.TestSlot.Room != null &&
                x.TestSlot.Room.StatusId ==
                RoomStatusIds.Active &&
                x.TestSlot.Room.Capacity > 0 &&
                x.StartTime >= x.TestSlot.StartTime &&
                x.EndTime <= x.TestSlot.EndTime &&
                (
                    x.TestSlot.SlotDate > today ||
                    (
                        x.TestSlot.SlotDate == today &&
                        x.StartTime > currentTime
                    )
                ))
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
                AssignedCount = unitOfWork.Context
                    .Set<TestSessionCandidate>()
                    .Count(testSessionCandidate =>
                        testSessionCandidate.TestSessionId == x.Id &&
                        testSessionCandidate.StatusId !=
                        TestSessionCandidateStatusIds.Cancelled &&
                        testSessionCandidate.StatusId !=
                        TestSessionCandidateStatusIds.Rescheduled)
            })
            .ToListAsync(cancellationToken);

        if (rawOptions.Count == 0)
        {
            return Result.Ok<IReadOnlyList<ExamResultCandidateRescheduleOptionDto>>([]);
        }

        var roomIds =
            rawOptions
                .Select(x => x.RoomId)
                .Distinct()
                .ToArray();

        var slotDates =
            rawOptions
                .Select(x => x.SlotDate)
                .Distinct()
                .ToArray();

        var reservationRows = await unitOfWork.Context
            .Set<TestSession>()
            .AsNoTracking()
            .Where(x =>
                x.TestSlot != null &&
                roomIds.Contains(x.TestSlot.RoomId) &&
                slotDates.Contains(x.TestSlot.SlotDate) &&
                x.StartTime.HasValue &&
                x.EndTime.HasValue &&
                !TestSessionCapacityService
                    .NonReservingStatusIds
                    .Contains(x.StatusId))
            .Select(x => new
            {
                x.TestSlot!.RoomId,
                x.TestSlot.SlotDate,
                StartTime = x.StartTime!.Value,
                EndTime = x.EndTime!.Value,
                CandidateCount = unitOfWork.Context
                    .Set<TestSessionCandidate>()
                    .Count(testSessionCandidate =>
                        testSessionCandidate.TestSessionId == x.Id &&
                        testSessionCandidate.Id != currentAssignmentId &&
                        testSessionCandidate.StatusId !=
                        TestSessionCandidateStatusIds.Cancelled &&
                        testSessionCandidate.StatusId !=
                        TestSessionCandidateStatusIds.Rescheduled)
            })
            .ToListAsync(cancellationToken);

        var reservationsByRoomDate =
            reservationRows
                .GroupBy(x => new { x.RoomId, x.SlotDate })
                .ToDictionary(
                    group => (
                        group.Key.RoomId,
                        group.Key.SlotDate),
                    group => group
                        .Select(x =>
                            new TestSessionCapacityReservation(
                                x.StartTime,
                                x.EndTime,
                                x.CandidateCount))
                        .ToArray());

        var options = rawOptions
            .Select(option =>
            {
                reservationsByRoomDate.TryGetValue(
                    (
                        option.RoomId,
                        option.SlotDate),
                    out var reservations);

                var occupied =
                    TestSessionCapacityService
                        .CalculatePeakOccupancy(
                            reservations ?? [],
                            option.StartTime!.Value,
                            option.EndTime!.Value);

                var available =
                    Math.Max(
                        0,
                        option.Capacity - occupied);

                return new { Option = option, AvailableCapacity = available };
            })
            .Where(x => x.AvailableCapacity > 0)
            .OrderBy(x => x.Option.SlotDate)
            .ThenBy(x => x.Option.StartTime)
            .ThenBy(x => x.Option.SessionNo)
            .Select(x =>
                new ExamResultCandidateRescheduleOptionDto(
                    x.Option.Id,
                    x.Option.SessionNo,
                    x.Option.TestSlotId,
                    x.Option.TitleAr,
                    x.Option.TitleEn,
                    x.Option.SlotDate,
                    x.Option.StartTime!.Value,
                    x.Option.EndTime!.Value,
                    x.Option.RoomId,
                    x.Option.RoomNameAr,
                    x.Option.RoomNameEn,
                    x.Option.Capacity,
                    x.Option.AssignedCount,
                    x.AvailableCapacity))
            .ToList();

        return Result.Ok<IReadOnlyList<ExamResultCandidateRescheduleOptionDto>>(options);
    }

    private static IResult<IReadOnlyList<ExamResultCandidateRescheduleOptionDto>> Failure(string code) =>
        Result.Fail<IReadOnlyList<ExamResultCandidateRescheduleOptionDto>>(new Error(code));
}
