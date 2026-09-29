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
using Application.Operation.Features.Employee.QuestionBankAssignments.Services;

namespace Application.Operation.Features.Employee.QuestionBankAssignments.Handlers;

public sealed class EditAssignmentQuestionCommandHandler(
    IUnitOfWork uow,
    ICurrentUserService currentUser,
    IRichTextSanitizer richTextSanitizer)
    : IRequestHandler<EditAssignmentQuestionCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(EditAssignmentQuestionCommand r, CancellationToken ct)
    {
        var questionInput = QuestionEntryRules.Sanitize(r.Question, richTextSanitizer);
        if (!QuestionEntryRules.Valid(questionInput, richTextSanitizer) ||
            !await QuestionEntryRules.ValidResource(uow, questionInput.ResourceId, ct))
            return Result.Fail<Unit>(ErrorsCodes.InvalidQuestionEntry);
        var employeeId = await AssignmentIdentity.CurrentEmployeeId(uow, currentUser, ct);
        if (employeeId is null) return Result.Fail<Unit>(ErrorsCodes.InvalidUserIdentifier);
        return await uow.ExecuteInTransactionAsync<IResult<Unit>>(async token =>
        {
            var item = await uow.GetEntityRepository<QuestionBankRequestItem>().DbSet
                .Include(x => x.QuestionBankAssignment).ThenInclude(x => x.QuestionBankRequest)
                .SingleOrDefaultAsync(x => x.Id == r.ItemId && x.QuestionBankAssignmentId == r.AssignmentId && !x.IsDeleted && x.StatusId == QuestionBankRequestItemStatusIds.DRAFT, token);
            if (item is null || item.QuestionBankAssignment.EmployeeId != employeeId || item.RequestId != item.QuestionBankAssignment.QuestionBankRequestId)
                return Result.Fail<Unit>(ErrorsCodes.QuestionBankAssignmentQuestionNotFound);
            if (!QuestionEntryRules.Editable(item.QuestionBankAssignment.StatusId) || item.QuestionBankAssignment.QuestionBankRequest.StatusId != QuestionBankRequestStatusIds.QuestionEntryInProgress)
                return Result.Fail<Unit>(ErrorsCodes.QuestionBankAssignmentNotEditable);
            var next = await uow.GetEntityRepository<QuestionRevision>().DbSet.Where(x => x.QuestionId == item.QuestionId).MaxAsync(x => (int?)x.RevisionNo, token) ?? 0;
            var revision = QuestionEntryRules.Revision(item.QuestionId, next + 1, item.Id, questionInput);
            await uow.GetEntityRepository<QuestionRevision>().AddAsync(revision, token); item.CurrentProposedRevisionId = revision.Id;
            await uow.SaveChangesAsync(token); return Result.Ok(Unit.Value);
        }, ct);
    }
}
