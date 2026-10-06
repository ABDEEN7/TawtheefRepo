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

            var originalRevisionId = await uow.GetEntityRepository<QuestionBankVersionQuestion>().DbSet.AsNoTracking()
                .Where(x => x.QuestionBankVersionId == assignment.QuestionBankRequest.BaseVersionId &&
                            x.QuestionId == command.QuestionId && !x.IsDeleted)
                .Select(x => (Guid?)x.QuestionRevisionId).SingleOrDefaultAsync(ct);
            if (originalRevisionId is null)
                return Result.Fail<Guid>(ErrorsCodes.QuestionBankBaseVersionQuestionNotFound);
            if (await uow.GetEntityRepository<QuestionBankRequestItem>().DbSet.AsNoTracking().AnyAsync(x =>
                    x.RequestId == assignment.QuestionBankRequestId && x.QuestionId == command.QuestionId && !x.IsDeleted, ct))
                return Result.Fail<Guid>(ErrorsCodes.QuestionBankQuestionAlreadyAssignedForMaintenance);

            var itemId = Guid.NewGuid();
            var nextRevisionNo = (await uow.GetEntityRepository<QuestionRevision>().DbSet
                .Where(x => x.QuestionId == command.QuestionId).MaxAsync(x => (int?)x.RevisionNo, ct) ?? 0) + 1;
            var revision = QuestionEntryRules.Revision(command.QuestionId, nextRevisionNo, itemId, input);
            var item = new QuestionBankRequestItem
            {
                Id = itemId, RequestId = assignment.QuestionBankRequestId,
                QuestionBankAssignmentId = assignment.Id, QuestionId = command.QuestionId,
                ChangeTypeId = QuestionChangeTypeIds.UPDATE, StatusId = QuestionBankRequestItemStatusIds.DRAFT,
                OriginalRevisionId = originalRevisionId, CurrentProposedRevisionId = revision.Id
            };
            await uow.GetEntityRepository<QuestionRevision>().AddAsync(revision, ct);
            await uow.GetEntityRepository<QuestionBankRequestItem>().AddAsync(item, ct);
            if (assignment.StatusId == QuestionBankAssignmentStatusIds.Assigned)
            {
                assignment.StatusId = QuestionBankAssignmentStatusIds.QuestionEntryInProgress;
                assignment.QuestionEntryStartedAt ??= DateTime.UtcNow;
            }
            try { await uow.SaveChangesAsync(ct); }
            catch (DbUpdateException)
            {
                return Result.Fail<Guid>(ErrorsCodes.QuestionBankQuestionAlreadyAssignedForMaintenance);
            }
            return Result.Ok(item.Id);
        }, cancellationToken);
    }
}
