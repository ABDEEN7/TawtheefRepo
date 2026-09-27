using Application.Operation.Features.Employee.Exams.ResultReport.DTOs;
using Application.Operation.Features.Employee.Exams.ResultReport.Queries;
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
            .Select(x => new { x.InvitationId, x.ResultStatusId,
                x.TestSessionCandidateId, x.TestAttemptId })
            .SingleOrDefaultAsync(cancellationToken);
        if (candidate is null)
            return Failure(ErrorsCodes.ExamResultCandidateNotFound);
        if (report.StatusId != ExamResultReportStatusIds.Returned)
            return Failure(ErrorsCodes.ExamResultReportNotReturned);
        if (candidate.ResultStatusId != ExamResultCandidateResultStatusIds.RequiresAction)
            return Failure(ErrorsCodes.ExamResultCandidateNotRequiresAction);
        if (candidate.TestSessionCandidateId is not Guid currentAssignmentId)
            return Failure(ErrorsCodes.ExamResultCandidateRescheduleStateInvalid);

        var currentAssignment = await unitOfWork.Context.Set<TestSessionCandidate>()
            .AsNoTracking()
            .Where(x => x.Id == currentAssignmentId)
            .Select(x => new { x.InvitationId, x.TestSessionId, x.StatusId,
                ExamId = x.TestSession!.ExamId,
                AttemptId = x.TestAttempt == null ? (Guid?)null : x.TestAttempt.Id })
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

        if (candidate.TestAttemptId is Guid attemptId)
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
                    interruptions[0].TestAttemptPartId is not Guid partId ||
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

        var now = timeProvider.GetLocalNow().DateTime;
        var today = DateOnly.FromDateTime(now);
        var currentTime = TimeOnly.FromDateTime(now);
        var options = await unitOfWork.Context.Set<TestSession>()
            .AsNoTracking()
            .Where(x => x.ExamId == report.ExamId &&
                x.Id != currentAssignment.TestSessionId &&
                (x.StatusId == TestSessionStatusIds.Approved ||
                 x.StatusId == TestSessionStatusIds.Ready) &&
                x.TestSlot != null && x.TestSlot.StatusId == TestSlotStatusIds.Ready &&
                x.TestSlot.Room != null && x.TestSlot.Room.StatusId == RoomStatusIds.Active &&
                x.TestSlot.Room.Capacity > 0 &&
                (x.TestSlot.SlotDate > today ||
                 x.TestSlot.SlotDate == today && x.TestSlot.StartTime > currentTime))
            .Select(x => new
            {
                x.Id,
                x.SessionNo,
                TestSlotId = x.TestSlot!.Id,
                x.TestSlot.TitleAr,
                x.TestSlot.TitleEn,
                x.TestSlot.SlotDate,
                x.TestSlot.StartTime,
                x.TestSlot.EndTime,
                RoomId = x.TestSlot.Room!.Id,
                RoomNameAr = x.TestSlot.Room.NameAr,
                RoomNameEn = x.TestSlot.Room.NameEn,
                x.TestSlot.Room.Capacity,
                AssignedCount = unitOfWork.Context.Set<TestSessionCandidate>().Count(assignment =>
                    assignment.TestSession!.TestSlotId == x.TestSlotId &&
                    assignment.StatusId != TestSessionCandidateStatusIds.Cancelled &&
                    assignment.StatusId != TestSessionCandidateStatusIds.Rescheduled)
            })
            .Where(x => x.AssignedCount < x.Capacity)
            .OrderBy(x => x.SlotDate)
            .ThenBy(x => x.StartTime)
            .ThenBy(x => x.SessionNo)
            .Select(x => new ExamResultCandidateRescheduleOptionDto(
                x.Id, x.SessionNo, x.TestSlotId, x.TitleAr, x.TitleEn,
                x.SlotDate, x.StartTime, x.EndTime, x.RoomId,
                x.RoomNameAr, x.RoomNameEn, x.Capacity, x.AssignedCount,
                x.Capacity - x.AssignedCount))
            .ToListAsync(cancellationToken);

        return Result.Ok<IReadOnlyList<ExamResultCandidateRescheduleOptionDto>>(options);
    }

    private static IResult<IReadOnlyList<ExamResultCandidateRescheduleOptionDto>> Failure(string code) =>
        Result.Fail<IReadOnlyList<ExamResultCandidateRescheduleOptionDto>>(new Error(code));
}
