using Application.Operation.Features.Employee.QuestionBankAssignments.Commands;
using Application.Operation.Features.Employee.QuestionBankAssignments.DTOs;
using Application.Operation.Features.Employee.QuestionBankAssignments.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.QuestionsBank;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.QuestionBankAssignments.Handlers;

public sealed class FinishQuestionBankAssignmentEntryCommandHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    : IRequestHandler<FinishQuestionBankAssignmentEntryCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(FinishQuestionBankAssignmentEntryCommand r, CancellationToken ct)
    {
        var employeeId = await AssignmentIdentity.CurrentEmployeeId(uow, currentUser, ct);
        if (employeeId is null) return Result.Fail<Unit>(ErrorsCodes.InvalidUserIdentifier);
        return await uow.ExecuteInTransactionAsync<IResult<Unit>>(async token =>
        {
            var a = await uow.GetEntityRepository<QuestionBankAssignment>().DbSet.Include(x => x.QuestionBankRequest)
                .Include(x => x.RequestItems.Where(i => !i.IsDeleted && i.StatusId != QuestionBankRequestItemStatusIds.REMOVED_FROM_REQUEST))
                .ThenInclude(i => i.CurrentProposedRevision).ThenInclude(x => x!.Options)
                .SingleOrDefaultAsync(x => x.Id == r.AssignmentId && x.EmployeeId == employeeId && !x.IsDeleted, token);
            if (a is null) return Result.Fail<Unit>(ErrorsCodes.QuestionBankAssignmentNotFound);
            if (!QuestionEntryRules.Editable(a.StatusId) || a.QuestionBankRequest.StatusId != QuestionBankRequestStatusIds.QuestionEntryInProgress)
                return Result.Fail<Unit>(ErrorsCodes.QuestionBankAssignmentNotEditable);
            var items = a.RequestItems.Where(x => x.RequestId == a.QuestionBankRequestId && x.StatusId == QuestionBankRequestItemStatusIds.DRAFT).ToList();
            if (items.Count < a.MinimumQuestionCount || items.Count == 0 || items.Any(x => x.CurrentProposedRevision == null || x.CurrentProposedRevision.Options.Count < 2 || x.CurrentProposedRevision.Options.Count(o => o.IsCorrect) != 1))
                return Result.Fail<Unit>(ErrorsCodes.QuestionBankMinimumQuestionCountNotMet);
            foreach (var item in items) item.StatusId = QuestionBankRequestItemStatusIds.PENDING_REVIEW;
            var now = DateTime.UtcNow; a.StatusId = QuestionBankAssignmentStatusIds.QuestionEntryCompleted; a.QuestionEntryCompletedAt = now;
            var unfinished = await uow.GetEntityRepository<QuestionBankAssignment>().DbSet.AsNoTracking().AnyAsync(x =>
                x.QuestionBankRequestId == a.QuestionBankRequestId && x.Id != a.Id && !x.IsDeleted && x.StatusId != QuestionBankAssignmentStatusIds.QuestionEntryCompleted, token);
            if (!unfinished)
            {
                a.QuestionBankRequest.StatusId = QuestionBankRequestStatusIds.PendingReview;
                await uow.GetEntityRepository<QuestionBankRequestHistory>().AddAsync(new QuestionBankRequestHistory { Id = Guid.NewGuid(), RequestId = a.QuestionBankRequestId,
                    FromStatusId = QuestionBankRequestStatusIds.QuestionEntryInProgress, ToStatusId = QuestionBankRequestStatusIds.PendingReview,
                    Action = "QuestionEntryCompleted", PerformedById = employeeId.Value, PerformedAt = now }, token);
            }
            await uow.SaveChangesAsync(token); return Result.Ok(Unit.Value);
        }, ct);
    }
}
