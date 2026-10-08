using Application.Operation.Features.Employee.QuestionBankAssignments.Commands;
using Application.Operation.Features.Employee.QuestionBankAssignments.Services;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.QuestionsBank;

namespace Application.Operation.Features.Employee.QuestionBankAssignments.Handlers;

public sealed class UpdateBaseVersionQuestionCommandHandler(
    IUnitOfWork uow, ICurrentUserService currentUser, IRichTextSanitizer sanitizer)
    : IRequestHandler<UpdateBaseVersionQuestionCommand, IResult<Guid>>
{
    public async Task<IResult<Guid>> Handle(UpdateBaseVersionQuestionCommand command, CancellationToken cancellationToken)
    {
        var input = QuestionEntryRules.Sanitize(command.Question, sanitizer);
        if (!QuestionEntryRules.Valid(input, sanitizer) ||
            !await QuestionEntryRules.ValidResource(uow, input.ResourceId, cancellationToken))
            return Result.Fail<Guid>(ErrorsCodes.InvalidQuestionEntry);
        var employeeId = await AssignmentIdentity.CurrentEmployeeId(uow, currentUser, cancellationToken);
        if (employeeId is null) return Result.Fail<Guid>(ErrorsCodes.InvalidUserIdentifier);

        return await uow.ExecuteInTransactionAsync<IResult<Guid>>(async ct =>
        {
            var assignment = await uow.GetEntityRepository<QuestionBankAssignment>().DbSet
                .Include(x => x.QuestionBankRequest)
                .SingleOrDefaultAsync(x => x.Id == command.AssignmentId && x.EmployeeId == employeeId && !x.IsDeleted, ct);
            if (assignment is null) return Result.Fail<Guid>(ErrorsCodes.QuestionBankAssignmentNotFound);
            if (assignment.QuestionBankRequest.RequestTypeId != QuestionBankRequestTypeIds.MAINTENANCE ||
                assignment.QuestionBankRequest.BaseVersionId is null ||
                !QuestionEntryRules.Editable(assignment.StatusId) ||
                assignment.QuestionBankRequest.StatusId != QuestionBankRequestStatusIds.QuestionEntryInProgress)
                return Result.Fail<Guid>(ErrorsCodes.QuestionBankAssignmentNotEditable);

            var original = await uow.GetEntityRepository<QuestionBankVersionQuestion>().DbSet.AsNoTracking()
                .Where(x => x.QuestionBankVersionId == assignment.QuestionBankRequest.BaseVersionId &&
                            x.QuestionId == command.QuestionId && !x.IsDeleted)
                .Select(x => new { x.QuestionRevisionId, x.QuestionRevision.QuestionTypeId })
                .SingleOrDefaultAsync(ct);
            if (original is null)
                return Result.Fail<Guid>(ErrorsCodes.QuestionBankBaseVersionQuestionNotFound);
            if (input.QuestionTypeId != original.QuestionTypeId)
                return Result.Fail<Guid>(ErrorsCodes.InvalidQuestionBankMaintenanceChange);
            var existing = await uow.GetEntityRepository<QuestionBankRequestItem>().DbSet
                .SingleOrDefaultAsync(x => x.RequestId == assignment.QuestionBankRequestId &&
                    x.QuestionId == command.QuestionId && !x.IsDeleted, ct);
            if (existing is not null && existing.StatusId != QuestionBankRequestItemStatusIds.REMOVED_FROM_REQUEST)
                return Result.Fail<Guid>(ErrorsCodes.QuestionBankQuestionAlreadyAssignedForMaintenance);

            var item = existing ?? new QuestionBankRequestItem
                { Id = Guid.NewGuid(), RequestId = assignment.QuestionBankRequestId, QuestionId = command.QuestionId };
            item.QuestionBankAssignmentId = assignment.Id;
            item.ChangeTypeId = QuestionChangeTypeIds.UPDATE;
            item.StatusId = QuestionBankRequestItemStatusIds.DRAFT;
            item.OriginalRevisionId = original.QuestionRevisionId;
            item.CurrentProposedRevisionId = null;
            item.RemovedById = null;
            item.RemovedAt = null;
            item.RemovalNote = null;
            if (existing is null)
                await uow.GetEntityRepository<QuestionBankRequestItem>().AddAsync(item, ct);
            if (assignment.StatusId == QuestionBankAssignmentStatusIds.Assigned)
            {
                assignment.StatusId = QuestionBankAssignmentStatusIds.QuestionEntryInProgress;
                assignment.QuestionEntryStartedAt ??= DateTime.UtcNow;
            }
            try
            {
                // Acquire the unique maintenance claim (or reclaim its row version) first.
                // No revision number is allocated until this save succeeds.
                await uow.SaveChangesAsync(ct);
                var nextRevisionNo = (await uow.GetEntityRepository<QuestionRevision>().DbSet
                    .Where(x => x.QuestionId == command.QuestionId).MaxAsync(x => (int?)x.RevisionNo, ct) ?? 0) + 1;
                var revision = QuestionEntryRules.Revision(command.QuestionId, nextRevisionNo, item.Id, input);
                item.CurrentProposedRevisionId = revision.Id;
                await uow.GetEntityRepository<QuestionRevision>().AddAsync(revision, ct);
                await uow.SaveChangesAsync(ct);
            }
            catch (DbUpdateException exception) when (MaintenanceClaimConflict.IsDuplicateQuestionClaim(exception))
            {
                return Result.Fail<Guid>(ErrorsCodes.QuestionBankQuestionAlreadyAssignedForMaintenance);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Result.Fail<Guid>(ErrorsCodes.QuestionBankQuestionAlreadyAssignedForMaintenance);
            }
            return Result.Ok(item.Id);
        }, cancellationToken);
    }
}
