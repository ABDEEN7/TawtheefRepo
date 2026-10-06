using Application.Operation.Features.Employee.QuestionBankAssignments.Commands;
using Application.Operation.Features.Employee.QuestionBankAssignments.Services;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.QuestionsBank;

namespace Application.Operation.Features.Employee.QuestionBankAssignments.Handlers;

public sealed class FinishQuestionBankAssignmentModificationCommandHandler(
    IUnitOfWork uow, ICurrentUserService currentUser, IRichTextSanitizer richTextSanitizer)
    : IRequestHandler<FinishQuestionBankAssignmentModificationCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(FinishQuestionBankAssignmentModificationCommand request, CancellationToken cancellationToken)
    {
        var employeeId = await AssignmentIdentity.CurrentEmployeeId(uow, currentUser, cancellationToken);
        if (employeeId is null) return Result.Fail<Unit>(ErrorsCodes.InvalidUserIdentifier);

        return await uow.ExecuteInTransactionAsync<IResult<Unit>>(async ct =>
        {
            var assignment = await uow.GetEntityRepository<QuestionBankAssignment>().DbSet
                .Include(x => x.QuestionBankRequest)
                .Include(x => x.RequestItems.Where(item => !item.IsDeleted))
                    .ThenInclude(item => item.CurrentProposedRevision).ThenInclude(revision => revision!.Options)
                .Include(x => x.RequestItems.Where(item => !item.IsDeleted))
                    .ThenInclude(item => item.OriginalRevision)
                .SingleOrDefaultAsync(x => x.Id == request.AssignmentId && x.EmployeeId == employeeId && !x.IsDeleted, ct);
            if (assignment is null) return Result.Fail<Unit>(ErrorsCodes.QuestionBankAssignmentNotFound);
            if (assignment.StatusId != QuestionBankAssignmentStatusIds.ReturnedForModification ||
                assignment.QuestionBankRequest.StatusId != QuestionBankRequestStatusIds.ModificationInProgress)
                return Result.Fail<Unit>(ErrorsCodes.QuestionBankAssignmentNotEditable);

            var items = assignment.RequestItems.Where(x => x.RequestId == assignment.QuestionBankRequestId).ToList();
            if (items.Any(x => x.StatusId == QuestionBankRequestItemStatusIds.NEEDS_MODIFICATION ||
                               x.StatusId == QuestionBankRequestItemStatusIds.REJECTED))
                return Result.Fail<Unit>(ErrorsCodes.InvalidQuestionEntry);

            var active = items.Where(x => x.StatusId != QuestionBankRequestItemStatusIds.REMOVED_FROM_REQUEST &&
                                          x.StatusId != QuestionBankRequestItemStatusIds.REJECTED).ToList();
            if (active.Count < assignment.MinimumQuestionCount || active.Count == 0)
                return Result.Fail<Unit>(ErrorsCodes.QuestionBankMinimumQuestionCountNotMet);

            var drafts = active.Where(x => x.StatusId == QuestionBankRequestItemStatusIds.DRAFT).ToList();
            if (drafts.Any(item => !Valid(item)))
                return Result.Fail<Unit>(ErrorsCodes.InvalidQuestionEntry);
            foreach (var draft in drafts)
                if (draft.ChangeTypeId != QuestionChangeTypeIds.DELETE &&
                    !await QuestionEntryRules.ValidResource(uow, draft.CurrentProposedRevision!.ResourceId, ct))
                    return Result.Fail<Unit>(ErrorsCodes.InvalidQuestionEntry);

            foreach (var item in drafts) item.StatusId = QuestionBankRequestItemStatusIds.PENDING_REVIEW;
            var now = DateTime.UtcNow;
            assignment.StatusId = QuestionBankAssignmentStatusIds.ModificationCompleted;
            assignment.LastModificationCompletedAt = now;

            var anotherReturnedAssignment = await uow.GetEntityRepository<QuestionBankAssignment>().DbSet.AsNoTracking()
                .AnyAsync(x => x.QuestionBankRequestId == assignment.QuestionBankRequestId && x.Id != assignment.Id &&
                               !x.IsDeleted && x.StatusId == QuestionBankAssignmentStatusIds.ReturnedForModification, ct);
            if (!anotherReturnedAssignment)
            {
                assignment.QuestionBankRequest.StatusId = QuestionBankRequestStatusIds.PendingReview;
                await uow.GetEntityRepository<QuestionBankRequestHistory>().AddAsync(new()
                {
                    Id = Guid.NewGuid(), RequestId = assignment.QuestionBankRequestId,
                    FromStatusId = QuestionBankRequestStatusIds.ModificationInProgress,
                    ToStatusId = QuestionBankRequestStatusIds.PendingReview,
                    Action = "QuestionModificationsCompleted", PerformedById = employeeId.Value, PerformedAt = now
                }, ct);
            }

            await uow.SaveChangesAsync(ct);
            return Result.Ok(Unit.Value);
        }, cancellationToken);
    }

    private bool Valid(QuestionBankRequestItem item)
    {
        if (item.ChangeTypeId == QuestionChangeTypeIds.DELETE)
            return item.OriginalRevisionId.HasValue && item.CurrentProposedRevisionId is null;
        var revision = item.CurrentProposedRevision;
        if (revision is null) return false;
        if (item.ChangeTypeId == QuestionChangeTypeIds.UPDATE &&
            item.OriginalRevision?.QuestionTypeId != revision.QuestionTypeId) return false;
        var input = new QuestionInput(revision.QuestionTypeId, revision.DifficultyLevelId,
            revision.QuestionTextAr, revision.QuestionTextEn, revision.ExplanationAr, revision.ExplanationEn,
            revision.ResourceId, revision.Options.Select(option => new QuestionOptionInput(option.OptionTextAr,
                option.OptionTextEn, option.IsCorrect, option.DisplayOrder)).ToList());
        return QuestionEntryRules.Valid(input, richTextSanitizer);
    }
}
