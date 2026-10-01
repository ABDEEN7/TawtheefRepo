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
                .SingleOrDefaultAsync(x => x.Id == r.ItemId && x.QuestionBankAssignmentId == r.AssignmentId && !x.IsDeleted && x.StatusId == QuestionBankRequestItemStatusIds.DRAFT, token);
            if (item is null || item.QuestionBankAssignment.EmployeeId != employeeId || item.RequestId != item.QuestionBankAssignment.QuestionBankRequestId)
                return Result.Fail<Unit>(ErrorsCodes.QuestionBankAssignmentQuestionNotFound);
            if (!QuestionEntryRules.Editable(item.QuestionBankAssignment.StatusId) || item.QuestionBankAssignment.QuestionBankRequest.StatusId != QuestionBankRequestStatusIds.QuestionEntryInProgress)
                return Result.Fail<Unit>(ErrorsCodes.QuestionBankAssignmentNotEditable);
            item.StatusId = QuestionBankRequestItemStatusIds.REMOVED_FROM_REQUEST; item.RemovedById = employeeId; item.RemovedAt = DateTime.UtcNow;
            await uow.SaveChangesAsync(token); return Result.Ok(Unit.Value);
        }, ct);
    }
}
