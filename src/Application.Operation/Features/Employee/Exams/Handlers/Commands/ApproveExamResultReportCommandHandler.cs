using System.Text.Json;
using Application.Operation.Features.Employee.Exams.Commands;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Logger;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Employee.Exams.Handlers.Commands;

public sealed class ApproveExamResultReportCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser)
    : IRequestHandler<
        ApproveExamResultReportCommand,
        IResult<Unit>>
{
    public Task<IResult<Unit>> Handle(
        ApproveExamResultReportCommand request,
        CancellationToken cancellationToken)
    {
        return unitOfWork.ExecuteInTransactionAsync<IResult<Unit>>(
            async token =>
            {
                if (!Guid.TryParse(
                        currentUser.UserId,
                        out var approverId))
                {
                    return Result.Fail<Unit>(
                        ErrorsCodes.InvalidRequest);
                }

                await unitOfWork.Context.Database
                    .ExecuteSqlRawAsync(
                        "DECLARE @result int; " +
                        "EXEC @result = sys.sp_getapplock " +
                        "@Resource = {0}, " +
                        "@LockMode = N'Exclusive', " +
                        "@LockOwner = N'Transaction', " +
                        "@LockTimeout = 15000; " +
                        "IF @result < 0 " +
                        "THROW 51000, " +
                        "'Exam result report approval lock unavailable', 1;",
                        [
                            $"Tawtheef.ExamResultReport.Approve.{request.ReportId}"
                        ],
                        token);

                var report = await unitOfWork.Context
                    .Set<ExamResultReport>()
                    .Include(x => x.Candidates)
                    .ThenInclude(x => x.Invitation)
                    .FirstOrDefaultAsync(
                        x => x.Id == request.ReportId,
                        token);

                if (report is null)
                {
                    return Result.Fail<Unit>(
                        new Error(
                            ErrorsCodes.ExamResultReportNotFound));
                }

                if (report.Candidates.Count == 0)
                {
                    return Result.Fail<Unit>(
                        new Error(
                            ErrorsCodes
                                .ExamResultReportHasNoCandidates));
                }

                var assignmentIds = report.Candidates
                    .Where(x =>
                        x.TestSessionCandidateId.HasValue)
                    .Select(x =>
                        x.TestSessionCandidateId!.Value)
                    .ToList();

                if (assignmentIds.Count > 0)
                {
                    var hasNonClosedSession =
                        await unitOfWork.Context
                            .Set<TestSessionCandidate>()
                            .AsNoTracking()
                            .AnyAsync(
                                x =>
                                    assignmentIds.Contains(x.Id) &&
                                    x.TestSession!.StatusId !=
                                    TestSessionStatusIds.Closed,
                                token);

                    if (hasNonClosedSession)
                    {
                        return Result.Fail<Unit>(
                            new Error(
                                ErrorsCodes
                                    .ExamResultReportSessionsNotClosed));
                    }
                }

                var qualifiedCandidates = report.Candidates
                    .Where(x => x.ResultStatusId == ExamResultCandidateResultStatusIds.QualifiedForInterview)
                    .ToList();

                foreach (var candidate in qualifiedCandidates)
                {
                    if (candidate.Invitation is null)
                    {
                        return Result.Fail<Unit>(new Error( ErrorsCodes.ExamResultReportInvitationStateInvalid));
                    }

                    // At this stage the candidate should still
                    // belong to the exam pipeline.
                    if (candidate.Invitation.InvitationStatusId != InvitationStatusIds.ExamEligible &&
                        candidate.Invitation.InvitationStatusId !=
                        InvitationStatusIds.InterviewEligible)
                    {
                        return Result.Fail<Unit>(new Error(ErrorsCodes.ExamResultReportInvitationStateInvalid));
                    }
                }

                var approveResult = report.Approve(approverId, request.DecisionNotes);

                if (approveResult.IsFailed)
                {
                    return Result.Fail<Unit>(approveResult.Errors);
                }

                foreach (var candidate in qualifiedCandidates)
                {
                    if (candidate.Invitation!.InvitationStatusId == InvitationStatusIds.InterviewEligible)
                    {
                        continue;
                    }

                    candidate.Invitation.ChangeInvitationStatus(InvitationStatusIds.InterviewEligible);
                }

                var closeResult = report.Close();

                if (closeResult.IsFailed)
                {
                    return Result.Fail<Unit>(closeResult.Errors);
                }

                report.AddDomainEvent(new Tawtheef.Domain.Events.Operation.Employee.Exams.ExamResultReportApprovedDomainEvent(
                    report.Id, report.ExamId, qualifiedCandidates.Count));

                await unitOfWork
                    .GetEntityRepository<ActionLog>()
                    .AddAsync(
                        new ActionLog
                        {
                            UserId = approverId,
                            LogType = ActionLogType.Employee,
                            ActionType = "ExamResultReportApproved",
                            Section = "ExamResultReport",
                            EntityId = report.Id,
                            Notes = JsonSerializer.Serialize(
                                new
                                {
                                    reportId = report.Id,
                                    examId = report.ExamId,
                                    approvedBy = approverId,
                                    approvedAt = report.ApprovedAt,
                                    candidateCount = report.Candidates.Count,
                                    qualifiedCount = qualifiedCandidates.Count,
                                    statuses = report.Candidates.GroupBy(x => x.ResultStatusId)
                                        .Select(x => new { statusId = x.Key, count = x.Count() }),
                                    decisionNotes = request.DecisionNotes
                                })
                        },
                        token);

                await unitOfWork.SaveChangesAsync(
                    token);

                return Result.Ok(Unit.Value);
            },
            cancellationToken);
    }
}
