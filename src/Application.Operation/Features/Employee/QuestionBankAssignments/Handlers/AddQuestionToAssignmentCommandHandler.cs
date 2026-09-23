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

public sealed class AddQuestionToAssignmentCommandHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    : IRequestHandler<AddQuestionToAssignmentCommand, IResult<Guid>>
{
    public async Task<IResult<Guid>> Handle(AddQuestionToAssignmentCommand r, CancellationToken ct)
    {
        if (!QuestionEntryRules.Valid(r.Question)) return Result.Fail<Guid>(ErrorsCodes.InvalidQuestionEntry);
        var employeeId = await AssignmentIdentity.CurrentEmployeeId(uow, currentUser, ct);
        if (employeeId is null) return Result.Fail<Guid>(ErrorsCodes.InvalidUserIdentifier);
        return await uow.ExecuteInTransactionAsync<IResult<Guid>>(async token =>
        {
            var a = await uow.GetEntityRepository<QuestionBankAssignment>().DbSet.Include(x => x.QuestionBankRequest)
                .SingleOrDefaultAsync(x => x.Id == r.AssignmentId && x.EmployeeId == employeeId && !x.IsDeleted, token);
            if (a is null) return Result.Fail<Guid>(ErrorsCodes.QuestionBankAssignmentNotFound);
            if (!QuestionEntryRules.Editable(a.StatusId) || a.QuestionBankRequest.StatusId != QuestionBankRequestStatusIds.QuestionEntryInProgress)
                return Result.Fail<Guid>(ErrorsCodes.QuestionBankAssignmentNotEditable);
            var question = new Question { Id = Guid.NewGuid() }; var itemId = Guid.NewGuid();
            var revision = QuestionEntryRules.Revision(question.Id, 1, null, r.Question);
            var item = new QuestionBankRequestItem { Id = itemId, RequestId = a.QuestionBankRequestId,
                QuestionBankAssignmentId = a.Id, QuestionId = question.Id, ChangeTypeId = QuestionChangeTypeIds.ADD,
                StatusId = QuestionBankRequestItemStatusIds.DRAFT, CurrentProposedRevisionId = revision.Id };
            await uow.GetEntityRepository<Question>().AddAsync(question, token);
            await uow.GetEntityRepository<QuestionRevision>().AddAsync(revision, token);
            await uow.GetEntityRepository<QuestionBankRequestItem>().AddAsync(item, token);
            if (a.StatusId == QuestionBankAssignmentStatusIds.Assigned) { a.StatusId = QuestionBankAssignmentStatusIds.QuestionEntryInProgress; a.QuestionEntryStartedAt ??= DateTime.UtcNow; }
            await uow.SaveChangesAsync(token); return Result.Ok(item.Id);
        }, ct);
    }
}
