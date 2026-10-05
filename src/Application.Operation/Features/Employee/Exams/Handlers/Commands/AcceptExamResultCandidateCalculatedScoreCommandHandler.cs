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

public sealed class AcceptExamResultCandidateCalculatedScoreCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    TimeProvider timeProvider,
    IApplicationLockService applicationLockService)
    : IRequestHandler<AcceptExamResultCandidateCalculatedScoreCommand, IResult<Unit>>
{
    public Task<IResult<Unit>> Handle(AcceptExamResultCandidateCalculatedScoreCommand request,
        CancellationToken cancellationToken) =>
        unitOfWork.ExecuteInTransactionAsync<IResult<Unit>>(async token =>
        {
            if (!Guid.TryParse(currentUser.UserId, out var employeeId))
                return Failure(ErrorsCodes.InvalidRequest);

            var notes = request.ResolutionNotes?.Trim();
            if (string.IsNullOrWhiteSpace(notes) || notes.Length > 2000)
                return Failure(ErrorsCodes.InvalidRequest);

            await applicationLockService.AcquireExclusiveAsync(
                ApplicationLockResources.ExamResultReportWorkflow(request.ReportId), token);

            var report = await unitOfWork.Context.Set<ExamResultReport>()
                .SingleOrDefaultAsync(x => x.Id == request.ReportId, token);
            if (report is null)
                return Failure(ErrorsCodes.ExamResultReportNotFound);
            if (report.StatusId != ExamResultReportStatusIds.Returned)
                return Failure(ErrorsCodes.ExamResultReportNotReturned);

            var candidate = await unitOfWork.Context.Set<ExamResultCandidate>()
                .SingleOrDefaultAsync(x => x.Id == request.CandidateResultId && x.ExamResultReportId == report.Id,
                    token);
            if (candidate is null)
                return Failure(ErrorsCodes.ExamResultCandidateNotFound);
            if (candidate.ResultStatusId != ExamResultCandidateResultStatusIds.RequiresAction)
                return Failure(ErrorsCodes.ExamResultCandidateNotRequiresAction);
            if (candidate.TestSessionCandidateId is not { } assignmentId
                || candidate.TestAttemptId is not { } attemptId)
                return Failure(ErrorsCodes.ExamResultCandidateAcceptScoreStateInvalid);

            await applicationLockService.AcquireExclusiveAsync(ApplicationLockResources.CandidateExamAttempt(attemptId),
                token);

            var assignment = await unitOfWork.Context.Set<TestSessionCandidate>()
                .AsNoTracking()
                .Where(x => x.Id == assignmentId)
                .Select(x => new
                {
                    x.Id,
                    x.InvitationId,
                    x.TestSession!.ExamId,
                    TestSessionStatusId = x.TestSession.StatusId
                })
                .SingleOrDefaultAsync(token);
            var attempt = await unitOfWork.Context.Set<TestAttempt>()
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == attemptId, token);
            if (assignment is null || assignment.InvitationId != candidate.InvitationId
                                   || assignment.ExamId != report.ExamId
                                   || assignment.TestSessionStatusId != TestSessionStatusIds.Closed || attempt is null
                                   || attempt.TestSessionCandidateId != assignment.Id
                                   || (attempt.StatusId != TestAttemptStatusIds.Completed
                                       && attempt.StatusId != TestAttemptStatusIds.TimedOut))
                return Failure(ErrorsCodes.ExamResultCandidateAcceptScoreStateInvalid);

            var openInterruptions = await unitOfWork.Context.Set<TestAttemptInterruption>()
                .Where(x => x.TestAttemptId == attemptId && x.StatusId == TestAttemptInterruptionStatusIds.Open)
                .Take(2)
                .ToListAsync(token);
            if (openInterruptions.Count == 0)
                return Failure(ErrorsCodes.ExamResultCandidateOpenInterruptionNotFound);
            if (openInterruptions.Count > 1)
                return Failure(ErrorsCodes.ExamResultCandidateMultipleOpenInterruptions);

            var interruption = openInterruptions[0];
            if (interruption.ResolvedAt is not null || interruption.ResolutionActionId is not null)
                return Failure(ErrorsCodes.ExamResultCandidateAcceptScoreStateInvalid);

            var previousInterruptionStatus = interruption.StatusId;
            var resolvedAt = timeProvider.GetUtcNow().UtcDateTime;
            interruption.StatusId = TestAttemptInterruptionStatusIds.Resolved;
            interruption.ResolutionActionId = TestAttemptInterruptionResolutionActionIds.AcceptCalculatedScore;
            interruption.ResolutionNotes = notes;
            interruption.ResolvedById = employeeId;
            interruption.ResolvedAt = resolvedAt;

            await unitOfWork.GetEntityRepository<ActionLog>()
                .AddAsync(
                    new ActionLog
                    {
                        UserId = employeeId,
                        LogType = ActionLogType.Employee,
                        ActionType = ExamResultReportAudit.CandidateCalculatedScoreAccepted,
                        Section = ExamResultReportAudit.Section,
                        EntityId = report.Id,
                        Notes = JsonSerializer.Serialize(new
                        {
                            reportId = report.Id,
                            examId = report.ExamId,
                            candidateResultId = candidate.Id,
                            invitationId = candidate.InvitationId,
                            testSessionCandidateId = assignment.Id,
                            testAttemptId = attempt.Id,
                            attemptStatus = attempt.StatusId,
                            interruptionId = interruption.Id,
                            previousInterruptionStatus,
                            newInterruptionStatus = interruption.StatusId,
                            resolutionAction = interruption.ResolutionActionId,
                            resolvedBy = employeeId,
                            resolvedAt,
                            resolutionNotes = notes,
                            reportStatus = report.StatusId,
                            resultStatus = candidate.ResultStatusId
                        })
                    }, token);

            await unitOfWork.SaveChangesAsync(token);
            return Result.Ok(Unit.Value);
        }, cancellationToken);

    private static IResult<Unit> Failure(string code) =>
        Result.Fail<Unit>(new Error(code));
}
