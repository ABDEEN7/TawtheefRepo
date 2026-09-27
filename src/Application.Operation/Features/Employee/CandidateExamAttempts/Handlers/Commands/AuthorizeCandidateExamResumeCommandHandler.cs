using Application.Operation.Features.Employee.CandidateExamAttempts.Commands;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Employee.CandidateExamAttempts.Handlers.Commands;

public sealed class AuthorizeCandidateExamResumeCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<AuthorizeCandidateExamResumeCommand, IResult<Unit>>
{
    public Task<IResult<Unit>> Handle(
        AuthorizeCandidateExamResumeCommand request,
        CancellationToken cancellationToken) =>
        unitOfWork.ExecuteInTransactionAsync(async token =>
        {
            await AcquireAttemptLockAsync(request.TestAttemptId, token);

            var testSlotId = await unitOfWork.Context.Set<TestAttempt>()
                .AsNoTracking()
                .Where(x => x.Id == request.TestAttemptId)
                .Select(x => (Guid?)x.TestSessionCandidate!.TestSession!.TestSlotId)
                .SingleOrDefaultAsync(token);
            if (testSlotId is null)
                return Unavailable();

            var isHallSupervisor = await unitOfWork.Context.Set<TestSlotStaff>()
                .AsNoTracking()
                .AnyAsync(staff => staff.TestSlotId == testSlotId.Value &&
                    staff.StaffUserId == request.OfficerId && staff.IsActive &&
                    staff.RoleId == TestSlotStaffRoleIds.HallSupervisor, token);
            if (!isHallSupervisor)
                return Result.Fail<Unit>(new Error(ErrorsCodes.UnauthorizedAction)
                    .WithMetadata("StatusCode", StatusCodes.Status403Forbidden));

            var attemptIsEligible = await unitOfWork.Context.Set<TestAttempt>()
                .AnyAsync(x => x.Id == request.TestAttemptId &&
                    x.StatusId == TestAttemptStatusIds.Interrupted &&
                    x.ActiveSessionId == null &&
                    x.TestSessionCandidate!.StatusId == TestSessionCandidateStatusIds.Started &&
                    x.TestSessionCandidate.TestSession!.StatusId == TestSessionStatusIds.InProgress &&
                    x.TestSessionCandidate.TestSession.TestSlot!.StatusId == TestSlotStatusIds.Started &&
                    (x.TestSessionCandidate.TestSession.Exam!.InterruptionPolicyId ==
                     ExamInterruptionPolicyIds.ResumeOnly ||
                     x.TestSessionCandidate.TestSession.Exam.InterruptionPolicyId ==
                     ExamInterruptionPolicyIds.ResumeOrReschedule), token);
            if (!attemptIsEligible)
                return Unavailable();

            var interruptedParts = await unitOfWork.Context.Set<TestAttemptPart>()
                .Where(x => x.TestAttemptId == request.TestAttemptId &&
                    x.StatusId == TestAttemptPartStatusIds.Interrupted)
                .Take(2)
                .ToListAsync(token);
            if (interruptedParts.Count != 1)
                return Unavailable();

            var interruptions = await unitOfWork.Context.Set<TestAttemptInterruption>()
                .Where(x => x.TestAttemptId == request.TestAttemptId &&
                    x.TestAttemptPartId == interruptedParts[0].Id &&
                    x.StatusId == TestAttemptInterruptionStatusIds.Open)
                .Take(2)
                .ToListAsync(token);
            if (interruptions.Count != 1)
                return Unavailable();

            var interruption = interruptions[0];
            if (interruption.ResolutionActionId == TestAttemptInterruptionResolutionActionIds.Resume &&
                interruption.ResolvedById is not null &&
                interruption.ResolvedAt is null)
            {
                return Result.Ok(Unit.Value);
            }

            if (interruption.ResolutionActionId is not null ||
                interruption.ResolvedById is not null ||
                interruption.ResolvedAt is not null)
            {
                return Unavailable();
            }

            interruption.ResolutionActionId = TestAttemptInterruptionResolutionActionIds.Resume;
            interruption.ResolvedById = request.OfficerId;
            interruption.ResolutionNotes = string.IsNullOrWhiteSpace(request.ResolutionNotes)
                ? null
                : request.ResolutionNotes.Trim();

            await unitOfWork.SaveChangesAsync(token);
            return Result.Ok(Unit.Value);
        }, cancellationToken);

    private Task AcquireAttemptLockAsync(Guid testAttemptId, CancellationToken cancellationToken) =>
        unitOfWork.Context.Database.ExecuteSqlRawAsync(
            "DECLARE @result int; EXEC @result = sys.sp_getapplock " +
            "@Resource = {0}, @LockMode = N'Exclusive', @LockOwner = N'Transaction', " +
            "@LockTimeout = 15000; IF @result < 0 THROW 51000, 'Candidate exam resume authorization lock unavailable', 1;",
            [$"Tawtheef.CandidateExam.Attempt.{testAttemptId}"], cancellationToken);

    private static IResult<Unit> Unavailable() =>
        Result.Fail<Unit>(ErrorsCodes.CandidateExamResumeAuthorizationUnavailable);
}
