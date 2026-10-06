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

public sealed class RemoveAssignmentQuestionCommandHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    : IRequestHandler<RemoveAssignmentQuestionCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(RemoveAssignmentQuestionCommand r, CancellationToken ct)
    {
        var employeeId = await AssignmentIdentity.CurrentEmployeeId(uow, currentUser, ct);
        if (employeeId is null) return Result.Fail<Unit>(ErrorsCodes.InvalidUserIdentifier);
        return await uow.ExecuteInTransactionAsync<IResult<Unit>>(async token =>
        {
            var item = await uow.GetEntityRepository<QuestionBankRequestItem>().DbSet.Include(x => x.QuestionBankAssignment).ThenInclude(x => x.QuestionBankRequest)
                .SingleOrDefaultAsync(x => x.Id == r.ItemId && x.QuestionBankAssignmentId == r.AssignmentId && !x.IsDeleted &&
                    (x.StatusId == QuestionBankRequestItemStatusIds.DRAFT ||
                     x.StatusId == QuestionBankRequestItemStatusIds.NEEDS_MODIFICATION ||
                     x.StatusId == QuestionBankRequestItemStatusIds.REJECTED), token);
            if (item is null)
            {
                var maintenanceItemExists = await uow.GetEntityRepository<QuestionBankRequestItem>().DbSet.AsNoTracking()
                    .AnyAsync(x => x.Id == r.ItemId && !x.IsDeleted &&
                        x.Request.RequestTypeId == QuestionBankRequestTypeIds.MAINTENANCE, token);
                return Result.Fail<Unit>(maintenanceItemExists
                    ? ErrorsCodes.QuestionBankMaintenanceItemNotOwned
                    : ErrorsCodes.QuestionBankAssignmentQuestionNotFound);
            }
            if (item.QuestionBankAssignment.EmployeeId != employeeId || item.RequestId != item.QuestionBankAssignment.QuestionBankRequestId)
                return Result.Fail<Unit>(ErrorsCodes.QuestionBankAssignmentQuestionNotFound);
            var initialEntry = item.StatusId == QuestionBankRequestItemStatusIds.DRAFT && QuestionEntryRules.Editable(item.QuestionBankAssignment.StatusId) &&
                               item.QuestionBankAssignment.QuestionBankRequest.StatusId == QuestionBankRequestStatusIds.QuestionEntryInProgress;
            var correction = (item.StatusId == QuestionBankRequestItemStatusIds.DRAFT ||
                              item.StatusId == QuestionBankRequestItemStatusIds.NEEDS_MODIFICATION ||
                              item.StatusId == QuestionBankRequestItemStatusIds.REJECTED) &&
                             QuestionEntryRules.CorrectionEditable(item.QuestionBankAssignment.StatusId) &&
                             item.QuestionBankAssignment.QuestionBankRequest.StatusId == QuestionBankRequestStatusIds.ModificationInProgress;
            if (!initialEntry && !correction)
                return Result.Fail<Unit>(ErrorsCodes.QuestionBankAssignmentNotEditable);
            if (item.QuestionBankAssignment.QuestionBankRequest.RequestTypeId == QuestionBankRequestTypeIds.MAINTENANCE &&
                item.OriginalRevisionId.HasValue && item.ChangeTypeId == QuestionChangeTypeIds.UPDATE)
            {
                item.ChangeTypeId = QuestionChangeTypeIds.DELETE;
                item.CurrentProposedRevisionId = null;
                item.StatusId = QuestionBankRequestItemStatusIds.DRAFT;
            }
            else
            {
                item.StatusId = QuestionBankRequestItemStatusIds.REMOVED_FROM_REQUEST;
                item.RemovedById = employeeId;
                item.RemovedAt = DateTime.UtcNow;
            }
            await uow.SaveChangesAsync(token); return Result.Ok(Unit.Value);
        }, ct);
    }
}
