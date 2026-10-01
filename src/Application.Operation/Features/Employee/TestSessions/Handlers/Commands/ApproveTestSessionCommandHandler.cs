using Application.Operation.Features.Employee.TestSessions.Commands;
using Application.Operation.Features.Employee.TestSessions.Services;
using Application.Operation.Features.Employee.TestSlots.DTOs;
using Application.Operation.Features.Employee.TestSlots.Handlers.Commands;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Logger;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Domain.Events.Operation.Employee.TestSessions;

namespace Application.Operation.Features.Employee.TestSessions.Handlers.Commands;

public sealed class ApproveTestSessionCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    : IRequestHandler<ApproveTestSessionCommand, IResult<Unit>>
{
    public Task<IResult<Unit>> Handle(ApproveTestSessionCommand request, CancellationToken ct) =>
        unitOfWork.ExecuteInTransactionAsync<IResult<Unit>>(token => ApproveAsync(request, token), ct);

    private async Task<IResult<Unit>> ApproveAsync(ApproveTestSessionCommand request, CancellationToken ct)
    {
        if (request.TestSessionId == Guid.Empty || !Guid.TryParse(currentUser.UserId, out var reviewerId))
            return ValidationFailure();

        var session = await unitOfWork.Context.Set<TestSession>().FirstOrDefaultAsync(x =>
            x.Id == request.TestSessionId && x.StatusId == TestSessionStatusIds.PendingApproval, ct);
        if (session is null)
            return ValidationFailure();

        var schedule = await unitOfWork.Context.Set<TestSlot>().AsNoTracking().Where(x => x.Id == session.TestSlotId)
            .Select(x => new { x.Id, x.RoomId, x.SlotDate, x.StartTime, x.EndTime, x.StatusId,
                Capacity = x.Room!.Capacity, RoomStatusId = x.Room.StatusId }).FirstOrDefaultAsync(ct);
        if (session.TestSlotId is null || session.StartTime is null || session.EndTime is null ||
            session.StartTime >= session.EndTime || schedule is null ||
            schedule.StatusId != TestSlotStatusIds.Ready || schedule.RoomStatusId != RoomStatusIds.Active)
            return ValidationFailure();

        await unitOfWork.Context.Database.ExecuteSqlRawAsync(
            "DECLARE @result int; EXEC @result = sys.sp_getapplock @Resource = {0}, " +
            "@LockMode = N'Exclusive', @LockOwner = N'Transaction', @LockTimeout = 15000; " +
            "IF @result < 0 THROW 51000, 'Test session room lock unavailable', 1;",
            [$"Tawtheef.TestSession.Room.{schedule.RoomId}.{schedule.SlotDate:yyyyMMdd}"], ct);

        var exam = await unitOfWork.Context.Set<Exam>().AsNoTracking().Where(x => x.Id == session.ExamId)
            .Select(x => new { x.Id, x.JobId, Duration = unitOfWork.Context.Set<ExamPart>()
                .Where(part => part.ExamId == x.Id).Sum(part => (int?)part.DurationMinutes) ?? 0 })
            .FirstOrDefaultAsync(ct);
        if (exam is null || exam.Duration <= 0 || session.StartTime < schedule.StartTime ||
            session.EndTime > schedule.EndTime ||
            (session.EndTime.Value - session.StartTime.Value).TotalMinutes < exam.Duration)
            return ValidationFailure();

        if (!TestSessionCapacityService.IsSessionStartCurrentOrFuture(
                schedule.SlotDate, session.StartTime.Value, DateTime.UtcNow))
            return SchedulingTimeFailure();

        var staff = await unitOfWork.Context.Set<TestSlotStaff>().AsNoTracking()
            .Where(x => x.TestSlotId == schedule.Id && x.IsActive).Select(x => new TestSlotStaffAssignmentDto
            { StaffUserId = x.StaffUserId, RoleId = x.RoleId, IsActive = x.IsActive }).ToListAsync(ct);
        if ((await TestSlotAssignmentOperations.ValidateAsync(unitOfWork, schedule.Id, schedule.SlotDate,
                schedule.StartTime, schedule.EndTime, staff, ct)).IsFailed)
            return ValidationFailure();

        var candidates = await unitOfWork.Context.Set<TestSessionCandidate>().AsNoTracking()
            .Where(x => x.TestSessionId == session.Id).Select(x => x.InvitationId).ToListAsync(ct);
        if (candidates.Count == 0 || candidates.Distinct().Count() != candidates.Count)
            return ValidationFailure();

        foreach (var invitationId in candidates.OrderBy(id => id))
        {
            await unitOfWork.Context.Database.ExecuteSqlRawAsync(
                "DECLARE @result int; EXEC @result = sys.sp_getapplock @Resource = {0}, " +
                "@LockMode = N'Exclusive', @LockOwner = N'Transaction', @LockTimeout = 15000; " +
                "IF @result < 0 THROW 51000, 'Test session candidate lock unavailable', 1;",
                [$"Tawtheef.TestSession.Candidate.{session.ExamId}.{invitationId}"], ct);
        }

        var validCandidateCount = await unitOfWork.Context.Set<Invitation>().AsNoTracking()
            .Where(x => candidates.Contains(x.Id) && x.JobId == exam.JobId)
            .Where(x => x.Source == InvitationSource.Exceptional ||
                x.InvitationStatusId == InvitationStatusIds.NewInvitation ||
                x.InvitationStatusId == InvitationStatusIds.Read ||
                (x.InvitationStatusId == InvitationStatusIds.ExamEligible && x.Applicant!.Profile != null &&
                 x.Applicant.Profile.Status == UserProfileStatus.Approved && x.Applicant.Profile.AvailableForRecruitment))
            .Where(x => !unitOfWork.Context.Set<TestSessionCandidate>().Any(candidate =>
                candidate.InvitationId == x.Id && candidate.TestSession!.ExamId == session.ExamId &&
                candidate.TestSessionId != session.Id && candidate.TestSession.StatusId != TestSessionStatusIds.Cancelled &&
                candidate.TestSession.StatusId != TestSessionStatusIds.Rejected)).CountAsync(ct);
        if (validCandidateCount != candidates.Count)
            return ValidationFailure();

        var reservations = await unitOfWork.Context.Set<TestSession>().AsNoTracking().Where(x =>
                x.TestSlot!.RoomId == schedule.RoomId && x.TestSlot.SlotDate == schedule.SlotDate && x.Id != session.Id &&
                !TestSessionCapacityService.NonReservingStatusIds.Contains(x.StatusId))
            .Select(x => new TestSessionCapacityReservation(x.StartTime!.Value, x.EndTime!.Value,
                unitOfWork.Context.Set<TestSessionCandidate>().Count(candidate => candidate.TestSessionId == x.Id)))
            .ToListAsync(ct);
        var availableSeats = Math.Max(0, schedule.Capacity - TestSessionCapacityService.CalculatePeakOccupancy(
            reservations, session.StartTime.Value, session.EndTime.Value));
        if (candidates.Count > availableSeats)
            return CapacityFailure(candidates.Count - availableSeats);

        var metadata = new TestSessionWorkflowMetadata(reviewerId, DateTime.UtcNow,
            nameof(TestSessionStatusIds.PendingApproval), nameof(TestSessionStatusIds.Approved));
        metadata.ApplyDecision(session, TestSessionStatusIds.Approved);
        await unitOfWork.GetEntityRepository<ActionLog>().AddAsync(metadata.CreateActionLog(session, "TestSessionApproved"), ct);
        if (session.CreatedById.HasValue)
            session.AddDomainEvent(new TestSessionApprovedDomainEvent(session.CreatedById.Value, session.Id, session.SessionNo));
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
    }

    private static IResult<Unit> ValidationFailure() => Result.Fail<Unit>(new Error("VALIDATION")
        .WithMetadata("Code", "Validation"));

    private static IResult<Unit> SchedulingTimeFailure() => Result.Fail<Unit>(
        new Error(ErrorsCodes.TestSessionScheduledTimeExpired).WithMetadata("Code", "Validation"));

    private static IResult<Unit> CapacityFailure(int count) => Result.Fail<Unit>(
        new Error(ErrorsCodes.TestSessionInsufficientCapacity).WithMetadata("Code", "Validation")
            .WithMetadata("UserMessage", $"TEST_SESSION_CAPACITY_INSUFFICIENT:{count}"));
}
