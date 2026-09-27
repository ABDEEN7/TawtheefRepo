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

public sealed class ReturnExamResultReportForProcessingCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser)
    : IRequestHandler<ReturnExamResultReportForProcessingCommand, IResult<Unit>>
{
    public Task<IResult<Unit>> Handle(
        ReturnExamResultReportForProcessingCommand request,
        CancellationToken cancellationToken)
    {
        return unitOfWork.ExecuteInTransactionAsync<IResult<Unit>>(
            async token =>
            {
                if (!Guid.TryParse(currentUser.UserId, out var returnedBy))
                    return Result.Fail<Unit>(ErrorsCodes.InvalidRequest);

                // Share the approval lock so both report decisions observe the latest state.
                await unitOfWork.Context.Database.ExecuteSqlRawAsync(
                    "DECLARE @result int; " +
                    "EXEC @result = sys.sp_getapplock " +
                    "@Resource = {0}, " +
                    "@LockMode = N'Exclusive', " +
                    "@LockOwner = N'Transaction', " +
                    "@LockTimeout = 15000; " +
                    "IF @result < 0 " +
                    "THROW 51000, 'Exam result report workflow lock unavailable', 1;",
                    [$"Tawtheef.ExamResultReport.Approve.{request.ReportId}"],
                    token);

                var report = await unitOfWork.Context.Set<ExamResultReport>()
                    .Include(x => x.Candidates)
                    .FirstOrDefaultAsync(x => x.Id == request.ReportId, token);

                if (report is null)
                    return Result.Fail<Unit>(new Error(ErrorsCodes.ExamResultReportNotFound));

                
                if (report.Candidates.All(x => x.ResultStatusId != ExamResultCandidateResultStatusIds.RequiresAction))
                {
                    return Result.Fail<Unit>(new Error(ErrorsCodes.ExamResultReportHasNoPendingActions));
                }
                
                var transition = report.ReturnForProcessing(request.Reason ?? string.Empty);
                if (transition.IsFailed)
                    return Result.Fail<Unit>(transition.Errors);

                await unitOfWork.GetEntityRepository<ActionLog>().AddAsync(
                    new ActionLog
                    {
                        UserId = returnedBy,
                        LogType = ActionLogType.Employee,
                        ActionType = "ExamResultReportReturnedForProcessing",
                        Section = "ExamResultReport",
                        EntityId = report.Id,
                        Notes = JsonSerializer.Serialize(new
                        {
                            reportId = report.Id,
                            examId = report.ExamId,
                            returnedBy,
                            returnedAt = DateTime.UtcNow,
                            reason = report.DecisionNotes,
                            candidateCount = report.Candidates.Count,
                            requiresActionCount = report.Candidates.Count(x =>
                                x.ResultStatusId == ExamResultCandidateResultStatusIds.RequiresAction)
                        })
                    },
                    token);

                await unitOfWork.SaveChangesAsync(token);
                return Result.Ok(Unit.Value);
            },
            cancellationToken);
    }
}
