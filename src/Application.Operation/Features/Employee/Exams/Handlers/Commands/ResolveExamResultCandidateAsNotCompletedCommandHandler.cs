using System.Text.Json;
using Application.Operation.Features.Employee.Exams.Commands;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Concurrency;
using Tawtheef.Application.Common.Interfaces.Concurrency;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Logger;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Employee.Exams.Handlers.Commands;

public sealed class ResolveExamResultCandidateAsNotCompletedCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    TimeProvider timeProvider,
    IApplicationLockService applicationLockService)
    : IRequestHandler<ResolveExamResultCandidateAsNotCompletedCommand, IResult<Unit>>
{
    public Task<IResult<Unit>> Handle(ResolveExamResultCandidateAsNotCompletedCommand request,
        CancellationToken cancellationToken) =>
        unitOfWork.ExecuteInTransactionAsync<IResult<Unit>>(async token =>
        {
            if (!Guid.TryParse(currentUser.UserId, out var employeeId))
                return Result.Fail<Unit>(ErrorsCodes.InvalidRequest);

            await applicationLockService.AcquireExclusiveAsync(
                ApplicationLockResources.ExamResultReportWorkflow(request.ReportId), token);

            var report = await unitOfWork.Context.Set<ExamResultReport>()
                .Include(x => x.Candidates)
                .FirstOrDefaultAsync(x => x.Id == request.ReportId, token);
            if (report is null)
                return Result.Fail<Unit>(new Error(ErrorsCodes.ExamResultReportNotFound));

            var candidate = report.Candidates.FirstOrDefault(x => x.Id == request.CandidateResultId);
            if (candidate is null)
                return Result.Fail<Unit>(new Error(ErrorsCodes.ExamResultCandidateNotFound));

            if (report.StatusId != ExamResultReportStatusIds.Returned)
                return Result.Fail<Unit>(new Error(ErrorsCodes.ExamResultReportNotReturned));

            if (candidate.ResultStatusId != ExamResultCandidateResultStatusIds.RequiresAction)
                return Result.Fail<Unit>(new Error(ErrorsCodes.ExamResultCandidateNotRequiresAction));

            var notes = request.ResolutionNotes?.Trim();
            if (string.IsNullOrWhiteSpace(notes))
                return Result.Fail<Unit>(new Error(ErrorsCodes.InvalidRequest));

            var resolvedAt = timeProvider.GetUtcNow().UtcDateTime;
            Guid? previousAttemptStatus = null;
            Guid? newAttemptStatus = null;

            if (candidate.TestAttemptId is { } attemptId)
            {
                await applicationLockService.AcquireExclusiveAsync(
                    ApplicationLockResources.CandidateExamAttempt(attemptId), token);

                var attempt = await unitOfWork.Context.Set<TestAttempt>()
                    .SingleOrDefaultAsync(x => x.Id == attemptId, token);
                if (attempt is null || candidate.TestSessionCandidateId != attempt.TestSessionCandidateId)
                    return InvalidState();

                previousAttemptStatus = attempt.StatusId;
                var openInterruptions = await unitOfWork.Context.Set<TestAttemptInterruption>()
                    .Where(x => x.TestAttemptId == attemptId && x.StatusId == TestAttemptInterruptionStatusIds.Open)
                    .Take(2)
                    .ToListAsync(token);

                if (attempt.StatusId == TestAttemptStatusIds.Interrupted)
                {
                    if (attempt.ActiveSessionId is not null || openInterruptions.Count != 1
                                                            || openInterruptions[0].ResolvedAt is not null
                                                            || openInterruptions[0].TestAttemptPartId is not
                                                                { } interruptedPartId)
                        return InvalidState();

                    var hasInterruptedPart = await unitOfWork.Context.Set<TestAttemptPart>()
                        .AnyAsync(
                            x => x.Id == interruptedPartId && x.TestAttemptId == attemptId
                                                           && x.StatusId == TestAttemptPartStatusIds.Interrupted,
                            token);
                    if (!hasInterruptedPart)
                        return InvalidState();

                    var interruption = openInterruptions[0];
                    interruption.Resolve(
                        TestAttemptInterruptionResolutionActionIds.Cancel,
                        notes,
                        employeeId,
                        resolvedAt);
                    attempt.CancelInterrupted();
                }
                else if (attempt.StatusId == TestAttemptStatusIds.Completed
                         || attempt.StatusId == TestAttemptStatusIds.TimedOut)
                {
                    if (openInterruptions.Count != 1 || openInterruptions[0].ResolvedAt is not null
                                                     || openInterruptions[0].ResolutionActionId is not null)
                        return InvalidState();

                    var interruption = openInterruptions[0];
                    interruption.Resolve(
                        TestAttemptInterruptionResolutionActionIds.Cancel,
                        notes,
                        employeeId,
                        resolvedAt);
                }
                else if ((attempt.StatusId != TestAttemptStatusIds.Cancelled
                          && attempt.StatusId != TestAttemptStatusIds.NotStarted) || attempt.ActiveSessionId is not null
                         || openInterruptions.Count != 0)
                {
                    return InvalidState();
                }

                newAttemptStatus = attempt.StatusId;
            }

            var previousResultStatus = candidate.ResultStatusId;
            candidate.MarkNotCompleted(resolvedAt);

            var remainingRequiresActionCount = report.Candidates.Count(x =>
                x.ResultStatusId == ExamResultCandidateResultStatusIds.RequiresAction);
            if (remainingRequiresActionCount == 0)
            {
                var transition = report.MarkProcessingCompleted();
                if (transition.IsFailed)
                    return Result.Fail<Unit>(transition.Errors);
            }

            await unitOfWork.GetEntityRepository<ActionLog>()
                .AddAsync(
                    new ActionLog
                    {
                        UserId = employeeId,
                        LogType = ActionLogType.Employee,
                        ActionType = ExamResultReportAudit.CandidateResolvedNotCompleted,
                        Section = ExamResultReportAudit.Section,
                        EntityId = report.Id,
                        Notes = JsonSerializer.Serialize(new
                        {
                            reportId = report.Id,
                            examId = report.ExamId,
                            candidateResultId = candidate.Id,
                            invitationId = candidate.InvitationId,
                            testSessionCandidateId = candidate.TestSessionCandidateId,
                            testAttemptId = candidate.TestAttemptId,
                            resolvedBy = employeeId,
                            resolvedAt,
                            resolutionNotes = notes,
                            previousResultStatus,
                            newResultStatus = candidate.ResultStatusId,
                            previousAttemptStatus,
                            newAttemptStatus,
                            remainingRequiresActionCount,
                            resultingReportStatus = report.StatusId
                        })
                    }, token);

            await unitOfWork.SaveChangesAsync(token);
            return Result.Ok(Unit.Value);
        }, cancellationToken);

    private static IResult<Unit> InvalidState() =>
        Result.Fail<Unit>(new Error(ErrorsCodes.ExamResultCandidateResolutionStateInvalid));
}
