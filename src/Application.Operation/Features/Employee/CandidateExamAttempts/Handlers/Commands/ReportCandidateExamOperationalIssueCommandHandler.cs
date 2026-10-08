using System.Text.Json;
using Application.Operation.Features.Employee.CandidateExamAttempts.Commands;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Concurrency;
using Tawtheef.Application.Common.Interfaces.Concurrency;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Logger;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Employee.CandidateExamAttempts.Handlers.Commands;

public sealed class ReportCandidateExamOperationalIssueCommandHandler(
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    IApplicationLockService applicationLockService)
    : IRequestHandler<ReportCandidateExamOperationalIssueCommand, IResult<Unit>>
{
    public Task<IResult<Unit>> Handle(ReportCandidateExamOperationalIssueCommand request,
        CancellationToken cancellationToken) =>
        unitOfWork.ExecuteInTransactionAsync<IResult<Unit>>(async token =>
        {
            var reason = request.Reason.Trim();

            if (string.IsNullOrWhiteSpace(reason))
                return Failure(ErrorsCodes.CandidateExamOperationalIssueReasonRequired);

            if (reason.Length > 2000)
                return Failure(ErrorsCodes.InvalidRequest);

            // Read only to discover the lock resource.
            var slotId = await unitOfWork.Context.Set<TestAttempt>()
                .AsNoTracking()
                .Where(x => x.Id == request.TestAttemptId)
                .Select(x => x.TestSessionCandidate!.TestSession!.TestSlotId)
                .SingleOrDefaultAsync(token);

            if (slotId is null)
                return Failure(ErrorsCodes.CandidateExamOperationalIssueStateInvalid);

            // Lock order must stay Slot -> Attempt.
            await applicationLockService.AcquireExclusiveAsync(
                ApplicationLockResources.CandidateExamTestSlot(slotId.Value), token);

            await applicationLockService.AcquireExclusiveAsync(
                ApplicationLockResources.CandidateExamAttempt(request.TestAttemptId), token);

            // Re-read AFTER locks.
            var attempt = await unitOfWork.Context.Set<TestAttempt>()
                .Include(x => x.TestSessionCandidate)
                .ThenInclude(x => x!.TestSession)
                .ThenInclude(x => x!.TestSlot)
                .SingleOrDefaultAsync(x => x.Id == request.TestAttemptId, token);

            if (attempt?.TestSessionCandidate?.TestSession?.TestSlot is not { } slot)
            {
                return Failure(ErrorsCodes.CandidateExamOperationalIssueStateInvalid);
            }

            var assignment = attempt.TestSessionCandidate;
            var session = assignment.TestSession!;

            // Hall operational issue can only be recorded before slot closing.
            if (slot.StatusId != TestSlotStatusIds.Started || session.StatusId != TestSessionStatusIds.InProgress
                                                           || assignment.StatusId
                                                           != TestSessionCandidateStatusIds.Completed
                                                           || attempt.ActiveSessionId is not null
                                                           || (attempt.StatusId != TestAttemptStatusIds.Completed
                                                               && attempt.StatusId != TestAttemptStatusIds.TimedOut))
            {
                return Failure(ErrorsCodes.CandidateExamOperationalIssueStateInvalid);
            }

            var isHallSupervisor = await unitOfWork.Context.Set<TestSlotStaff>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.TestSlotId == slot.Id && x.StaffUserId == request.OfficerId && x.IsActive
                         && x.RoleId == TestSlotStaffRoleIds.HallSupervisor, token);

            if (!isHallSupervisor)
            {
                return Result.Fail<Unit>(
                    new Error(ErrorsCodes.UnauthorizedAction).WithMetadata("StatusCode",
                        StatusCodes.Status403Forbidden));
            }

            var hasOpenIssue = await unitOfWork.Context.Set<TestAttemptInterruption>()
                .AsNoTracking()
                .AnyAsync(x => x.TestAttemptId == attempt.Id && x.StatusId == TestAttemptInterruptionStatusIds.Open,
                    token);

            if (hasOpenIssue)
                return Failure(ErrorsCodes.CandidateExamOperationalIssueAlreadyOpen);

            var now = timeProvider.GetUtcNow().UtcDateTime;

            var interruption = new TestAttemptInterruption
            {
                Id = Guid.NewGuid(),
                TestAttemptId = attempt.Id,

                // NULL intentionally:
                // this issue applies to the terminal attempt as a whole,
                // not an active interrupted part.
                TestAttemptPartId = null,
                InterruptedAt = now,
                Reason = reason,
                StatusId = TestAttemptInterruptionStatusIds.Open,
                ResolutionActionId = null,
                ResolutionNotes = null,
                ResolvedById = null,
                ResolvedAt = null
            };

            await unitOfWork.Context.Set<TestAttemptInterruption>().AddAsync(interruption, token);

            attempt.AddDomainEvent(
                new Tawtheef.Domain.Events.Operation.Employee.Exams.CandidateExamOperationalIssueReportedDomainEvent(
                    interruption.Id, attempt.Id, slot.Id));

            await unitOfWork.GetEntityRepository<ActionLog>()
                .AddAsync(
                    new ActionLog
                    {
                        UserId = request.OfficerId,
                        LogType = ActionLogType.Employee,
                        ActionType = CandidateExamAttemptAudit.OperationalIssueReported,
                        Section = CandidateExamAttemptAudit.Section,
                        EntityId = attempt.Id,
                        Notes = JsonSerializer.Serialize(new
                        {
                            testAttemptId = attempt.Id,
                            testSessionCandidateId = assignment.Id,
                            testSessionId = session.Id,
                            testSlotId = slot.Id,
                            attemptStatus = attempt.StatusId,
                            interruptionId = interruption.Id,
                            reason,
                            reportedBy = request.OfficerId,
                            reportedAt = now
                        })
                    }, token);

            await unitOfWork.SaveChangesAsync(token);

            return Result.Ok(Unit.Value);
        }, cancellationToken);

    private static IResult<Unit> Failure(string code) =>
        Result.Fail<Unit>(new Error(code));
}
