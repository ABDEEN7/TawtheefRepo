using Application.Operation.Features.Employee.QuestionBankAssignments.Commands;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.QuestionsBank;

namespace Application.Operation.Features.Employee.QuestionBankAssignments.Handlers;

public sealed class DeleteBaseVersionQuestionCommandHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    : IRequestHandler<DeleteBaseVersionQuestionCommand, IResult<Guid>>
{
    public async Task<IResult<Guid>> Handle(DeleteBaseVersionQuestionCommand command, CancellationToken cancellationToken)
    {
        var employeeId = await AssignmentIdentity.CurrentEmployeeId(uow, currentUser, cancellationToken);
        if (employeeId is null) return Result.Fail<Guid>(ErrorsCodes.InvalidUserIdentifier);
        return await uow.ExecuteInTransactionAsync<IResult<Guid>>(async ct =>
        {
            var assignment = await uow.GetEntityRepository<QuestionBankAssignment>().DbSet
                .Include(x => x.QuestionBankRequest)
                .SingleOrDefaultAsync(x => x.Id == command.AssignmentId && x.EmployeeId == employeeId && !x.IsDeleted, ct);
            if (assignment is null) return Result.Fail<Guid>(ErrorsCodes.QuestionBankAssignmentNotFound);
            if (assignment.QuestionBankRequest.RequestTypeId != QuestionBankRequestTypeIds.MAINTENANCE ||
                assignment.QuestionBankRequest.BaseVersionId is null)
                return Result.Fail<Guid>(ErrorsCodes.QuestionBankAssignmentNotEditable);
            var initialEntry = QuestionEntryRules.Editable(assignment.StatusId) &&
                               assignment.QuestionBankRequest.StatusId == QuestionBankRequestStatusIds.QuestionEntryInProgress;
            var correction = QuestionEntryRules.CorrectionEditable(assignment.StatusId) &&
                             assignment.QuestionBankRequest.StatusId == QuestionBankRequestStatusIds.ModificationInProgress;
            if (!initialEntry && !correction)
                return Result.Fail<Guid>(ErrorsCodes.QuestionBankAssignmentNotEditable);
            var originalRevisionId = await uow.GetEntityRepository<QuestionBankVersionQuestion>().DbSet.AsNoTracking()
                .Where(x => x.QuestionBankVersionId == assignment.QuestionBankRequest.BaseVersionId &&
                            x.QuestionId == command.QuestionId && !x.IsDeleted)
                .Select(x => (Guid?)x.QuestionRevisionId).SingleOrDefaultAsync(ct);
            if (originalRevisionId is null)
                return Result.Fail<Guid>(ErrorsCodes.QuestionBankBaseVersionQuestionNotFound);
            var existing = await uow.GetEntityRepository<QuestionBankRequestItem>().DbSet
                .SingleOrDefaultAsync(x => x.RequestId == assignment.QuestionBankRequestId &&
                    x.QuestionId == command.QuestionId && !x.IsDeleted, ct);
            if (existing is not null)
            {
                if (existing.QuestionBankAssignmentId != assignment.Id)
                    return Result.Fail<Guid>(ErrorsCodes.QuestionBankQuestionAlreadyAssignedForMaintenance);
                if (existing.ChangeTypeId != QuestionChangeTypeIds.UPDATE &&
                    existing.ChangeTypeId != QuestionChangeTypeIds.DELETE)
                    return Result.Fail<Guid>(ErrorsCodes.InvalidQuestionBankMaintenanceChange);
                var statusAllowed = initialEntry
                    ? existing.StatusId == QuestionBankRequestItemStatusIds.DRAFT
                    : existing.StatusId == QuestionBankRequestItemStatusIds.DRAFT ||
                      existing.StatusId == QuestionBankRequestItemStatusIds.NEEDS_MODIFICATION;
                if (!statusAllowed)
                    return Result.Fail<Guid>(ErrorsCodes.QuestionBankAssignmentNotEditable);
                existing.ChangeTypeId = QuestionChangeTypeIds.DELETE;
                existing.OriginalRevisionId = originalRevisionId;
                existing.CurrentProposedRevisionId = null;
                existing.StatusId = QuestionBankRequestItemStatusIds.DRAFT;
                existing.RemovedAt = null;
                existing.RemovedById = null;
                await uow.SaveChangesAsync(ct);
                return Result.Ok(existing.Id);
            }

            var item = new QuestionBankRequestItem
            {
                Id = Guid.NewGuid(), RequestId = assignment.QuestionBankRequestId,
                QuestionBankAssignmentId = assignment.Id, QuestionId = command.QuestionId,
                ChangeTypeId = QuestionChangeTypeIds.DELETE, StatusId = QuestionBankRequestItemStatusIds.DRAFT,
                OriginalRevisionId = originalRevisionId, CurrentProposedRevisionId = null
            };
            await uow.GetEntityRepository<QuestionBankRequestItem>().AddAsync(item, ct);
            if (assignment.StatusId == QuestionBankAssignmentStatusIds.Assigned)
            {
                assignment.StatusId = QuestionBankAssignmentStatusIds.QuestionEntryInProgress;
                assignment.QuestionEntryStartedAt ??= DateTime.UtcNow;
            }
            try { await uow.SaveChangesAsync(ct); }
            catch (DbUpdateException exception) when (MaintenanceClaimConflict.IsDuplicateQuestionClaim(exception))
            {
                return Result.Fail<Guid>(ErrorsCodes.QuestionBankQuestionAlreadyAssignedForMaintenance);
            }
            return Result.Ok(item.Id);
        }, cancellationToken);
    }
}
