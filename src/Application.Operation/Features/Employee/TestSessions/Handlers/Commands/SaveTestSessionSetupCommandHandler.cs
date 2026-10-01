using Application.Operation.Features.Employee.TestSessions.Commands;
using Application.Operation.Features.Employee.TestSessions.DTOs;
using Application.Operation.Features.Employee.TestSessions.Services;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.TestSessions.Handlers.Commands;

public sealed class SaveTestSessionSetupCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<SaveTestSessionSetupCommand, IResult<SavedTestSessionSetupDto>>
{
    public Task<IResult<SavedTestSessionSetupDto>> Handle(
        SaveTestSessionSetupCommand request,
        CancellationToken ct)
        => unitOfWork.ExecuteInTransactionAsync<IResult<SavedTestSessionSetupDto>>(
            token => SaveAsync(request.Setup, token), ct);

    private async Task<IResult<SavedTestSessionSetupDto>> SaveAsync(
        SaveTestSessionSetupDto setup,
        CancellationToken ct)
    {
        var suppliedInvitationIds = setup.InvitationIds ?? [];
        var invitationIds = suppliedInvitationIds.Distinct().ToArray();
        var hasCompleteSchedule = setup.TestSlotId.HasValue && setup.StartTime.HasValue &&
                                  setup.EndTime.HasValue;
        var candidateIdsToPersist = hasCompleteSchedule || setup.SendToApprove ? invitationIds : [];
        if (invitationIds.Length != suppliedInvitationIds.Count ||
            (setup.TestSlotId.HasValue || setup.StartTime.HasValue || setup.EndTime.HasValue) &&
            !hasCompleteSchedule ||
            hasCompleteSchedule && setup.StartTime >= setup.EndTime ||
            setup.SendToApprove && (!hasCompleteSchedule || invitationIds.Length == 0))
            return ValidationFailure();

        var exam = await unitOfWork.Context.Set<Exam>().AsNoTracking()
            .Where(x => x.Id == setup.ExamId)
            .Select(x => new { x.Id, x.JobId, x.StatusId, Duration = unitOfWork.Context.Set<ExamPart>()
                .Where(part => part.ExamId == x.Id).Sum(part => (int?)part.DurationMinutes) ?? 0 })
            .FirstOrDefaultAsync(ct);
        if (exam is null || setup.SendToApprove &&
            (exam.StatusId != ExamStatusIds.Approved || exam.Duration <= 0))
            return ValidationFailure();

        var session = setup.TestSessionId.HasValue
            ? await unitOfWork.Context.Set<TestSession>()
                .FirstOrDefaultAsync(x => x.Id == setup.TestSessionId && x.ExamId == setup.ExamId, ct)
            : null;
        if (setup.TestSessionId.HasValue &&
            (session is null || !IsEditable(session.StatusId)))
            return ValidationFailure();

        var slot = hasCompleteSchedule
            ? await unitOfWork.Context.Set<TestSlot>().AsNoTracking()
                .Where(x => x.Id == setup.TestSlotId)
                .Select(x => new
                {
                    x.Id,
                    x.RoomId,
                    x.SlotDate,
                    x.StartTime,
                    x.EndTime,
                    x.StatusId,
                    Capacity = x.Room!.Capacity,
                    RoomStatusId = x.Room.StatusId
                }).FirstOrDefaultAsync(ct)
            : null;

        if (hasCompleteSchedule && (exam.Duration <= 0 || slot is null ||
            slot.StatusId != TestSlotStatusIds.Ready || slot.RoomStatusId != RoomStatusIds.Active ||
            setup.StartTime!.Value < slot.StartTime || setup.EndTime!.Value > slot.EndTime ||
            (setup.EndTime.Value - setup.StartTime.Value).TotalMinutes < exam.Duration))
            return ValidationFailure();

        if (setup.SendToApprove && slot is not null &&
            !TestSessionCapacityService.IsSessionStartCurrentOrFuture(
                slot.SlotDate, setup.StartTime!.Value, DateTime.UtcNow))
            return SchedulingTimeFailure();

        if (setup.SendToApprove && slot is not null)
        {
            await unitOfWork.Context.Database.ExecuteSqlRawAsync(
                "DECLARE @result int; EXEC @result = sys.sp_getapplock " +
                "@Resource = {0}, @LockMode = N'Exclusive', " +
                "@LockOwner = N'Transaction', @LockTimeout = 15000; " +
                "IF @result < 0 THROW 51000, 'Test session room lock unavailable', 1;",
                [$"Tawtheef.TestSession.Room.{slot.RoomId}.{slot.SlotDate:yyyyMMdd}"], ct);
        }

        if (session is null)
        {
            await unitOfWork.Context.Database.ExecuteSqlRawAsync(
                "DECLARE @result int; EXEC @result = sys.sp_getapplock " +
                "@Resource = {0}, @LockMode = N'Exclusive', " +
                "@LockOwner = N'Transaction', @LockTimeout = 15000; " +
                "IF @result < 0 THROW 51000, 'Test session numbering lock unavailable', 1;",
                [$"Tawtheef.TestSession.Numbering.{(slot?.SlotDate.Year ?? DateTime.UtcNow.Year)}"], ct);
        }

        foreach (var invitationId in candidateIdsToPersist.OrderBy(id => id))
        {
            await unitOfWork.Context.Database.ExecuteSqlRawAsync(
                "DECLARE @result int; EXEC @result = sys.sp_getapplock " +
                "@Resource = {0}, @LockMode = N'Exclusive', " +
                "@LockOwner = N'Transaction', @LockTimeout = 15000; " +
                "IF @result < 0 THROW 51000, 'Test session candidate lock unavailable', 1;",
                [$"Tawtheef.TestSession.Candidate.{setup.ExamId}.{invitationId}"], ct);
        }

        var validCandidateCount = await ValidCandidateQuery(setup, exam.JobId, candidateIdsToPersist)
            .CountAsync(ct);
        if (validCandidateCount != candidateIdsToPersist.Length)
            return ValidationFailure();

        var duplicateCandidates = await unitOfWork.Context.Set<TestSessionCandidate>().AsNoTracking()
            .Where(candidate => candidateIdsToPersist.Contains(candidate.InvitationId) &&
                candidate.TestSession!.ExamId == setup.ExamId &&
                candidate.TestSessionId != (session == null ? Guid.Empty : session.Id) &&
                candidate.TestSession.StatusId != TestSessionStatusIds.Cancelled &&
                candidate.TestSession.StatusId != TestSessionStatusIds.Rejected)
            .AnyAsync(ct);
        if (duplicateCandidates) return ValidationFailure();

        if (slot is not null)
        {
            var reservations = await unitOfWork.Context.Set<TestSession>().AsNoTracking()
                .Where(existing => existing.TestSlot!.RoomId == slot.RoomId &&
                    existing.TestSlot.SlotDate == slot.SlotDate &&
                    existing.Id != (session == null ? Guid.Empty : session.Id) &&
                    !TestSessionCapacityService.NonReservingStatusIds.Contains(existing.StatusId))
                .Select(existing => new TestSessionCapacityReservation(existing.StartTime!.Value, existing.EndTime!.Value,
                    unitOfWork.Context.Set<TestSessionCandidate>().Count(candidate =>
                        candidate.TestSessionId == existing.Id)))
                .ToListAsync(ct);
            var occupied = TestSessionCapacityService.CalculatePeakOccupancy(
                reservations, setup.StartTime!.Value, setup.EndTime!.Value);
            var availableSeats = Math.Max(0, slot.Capacity - occupied);
            if (candidateIdsToPersist.Length > availableSeats)
                return CapacityFailure(candidateIdsToPersist.Length - availableSeats);
        }

        if (session is null)
        {
            session = new TestSession
            {
                Id = Guid.NewGuid(),
                ExamId = setup.ExamId,
                TestSlotId = setup.TestSlotId,
                SessionNo = await GenerateSessionNoAsync(slot?.SlotDate.Year ?? DateTime.UtcNow.Year, ct),
                StatusId = TestSessionStatusIds.Draft
            };
            await unitOfWork.Context.Set<TestSession>().AddAsync(session, ct);
        }

        session.TestSlotId = setup.TestSlotId;
        session.StartTime = setup.StartTime;
        session.EndTime = setup.EndTime;
        session.GenderFilter = setup.GenderFilter;
        session.NationalityFilter = setup.NationalityFilter;
        session.StatusId = setup.SendToApprove
            ? TestSessionStatusIds.PendingApproval
            : session.StatusId;

        var currentCandidates = await unitOfWork.Context.Set<TestSessionCandidate>()
            .Where(candidate => candidate.TestSessionId == session.Id).ToListAsync(ct);
        var deselected = currentCandidates
            .Where(candidate => !candidateIdsToPersist.Contains(candidate.InvitationId)).ToList();
        unitOfWork.RemoveRange(deselected);
        var existingIds = currentCandidates.Select(candidate => candidate.InvitationId).ToHashSet();
        var additions = candidateIdsToPersist.Where(id => !existingIds.Contains(id)).Select(id => new TestSessionCandidate
        {
            Id = Guid.NewGuid(),
            TestSessionId = session.Id,
            InvitationId = id,
            AttendanceStatusId = TestSessionCandidateAttendanceStatusIds.Pending,
            IdentityVerificationStatusId = TestSessionCandidateIdentityVerificationStatusIds.Pending,
            StatusId = TestSessionCandidateStatusIds.Assigned
        }).ToList();
        await unitOfWork.Context.Set<TestSessionCandidate>().AddRangeAsync(additions, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Ok(new SavedTestSessionSetupDto(
            session.Id, session.SessionNo, session.StatusId, candidateIdsToPersist.Length));
    }

    private IQueryable<Invitation> ValidCandidateQuery(
        SaveTestSessionSetupDto setup,
        Guid jobId,
        Guid[] invitationIds)
    {
        var query = unitOfWork.Context.Set<Invitation>().AsNoTracking()
            .Where(invitation => invitationIds.Contains(invitation.Id) && invitation.JobId == jobId)
            .Where(invitation => invitation.Source == InvitationSource.Exceptional ||
                invitation.InvitationStatusId == InvitationStatusIds.NewInvitation ||
                invitation.InvitationStatusId == InvitationStatusIds.Read ||
                (invitation.InvitationStatusId == InvitationStatusIds.ExamEligible &&
                 invitation.Applicant!.Profile != null &&
                 invitation.Applicant.Profile.Status == UserProfileStatus.Approved &&
                 invitation.Applicant.Profile.AvailableForRecruitment))
            .Where(invitation => !unitOfWork.Context.Set<TestSessionCandidate>().Any(candidate =>
                candidate.InvitationId == invitation.Id &&
                candidate.TestSession!.ExamId == setup.ExamId &&
                candidate.TestSessionId != (setup.TestSessionId ?? Guid.Empty) &&
                candidate.TestSession.StatusId != TestSessionStatusIds.Cancelled &&
                candidate.TestSession.StatusId != TestSessionStatusIds.Rejected));

        if (setup.GenderFilter.HasValue)
            query = query.Where(invitation => invitation.Applicant!.Profile!.GenderId ==
                (setup.GenderFilter == TestSessionGenderFilter.Male ? GenderIds.Male : GenderIds.Female));
        if (setup.NationalityFilter.HasValue)
            query = setup.NationalityFilter == TestSessionNationalityFilter.Qatari
                ? query.Where(invitation => invitation.Applicant!.Profile!.NationalityId == CountryIds.Qatar)
                : query.Where(invitation => invitation.Applicant!.Profile!.NationalityId.HasValue &&
                    invitation.Applicant.Profile.NationalityId != CountryIds.Qatar);
        return query;
    }

    private async Task<string> GenerateSessionNoAsync(int year, CancellationToken ct)
    {
        var prefix = $"TS-{year}-";
        var numbers = await unitOfWork.Context.Set<TestSession>().AsNoTracking()
            .Where(session => session.SessionNo.StartsWith(prefix))
            .Select(session => session.SessionNo).ToListAsync(ct);
        var next = numbers.Select(number => int.TryParse(number[prefix.Length..], out var value) ? value : 0)
            .DefaultIfEmpty().Max() + 1;
        if (next > 9999) throw new InvalidOperationException("Test session sequence exceeded 9999.");
        return $"{prefix}{next:D4}";
    }

    private static IResult<SavedTestSessionSetupDto> ValidationFailure()
        => Result.Fail<SavedTestSessionSetupDto>(new Error("VALIDATION")
            .WithMetadata("Code", "Validation"));

    private static IResult<SavedTestSessionSetupDto> SchedulingTimeFailure()
        => Result.Fail<SavedTestSessionSetupDto>(new Error(ErrorsCodes.TestSessionScheduledTimeExpired)
            .WithMetadata("Code", "Validation"));

    private static bool IsEditable(Guid statusId) =>
        statusId == TestSessionStatusIds.Draft || statusId == TestSessionStatusIds.Returned;

    private static IResult<SavedTestSessionSetupDto> CapacityFailure(int count)
        => Result.Fail<SavedTestSessionSetupDto>(new Error(ErrorsCodes.TestSessionInsufficientCapacity)
            .WithMetadata("Code", "Validation")
            .WithMetadata("UserMessage", $"TEST_SESSION_CAPACITY_INSUFFICIENT:{count}"));
}
