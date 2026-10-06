using Application.Operation.Features.Employee.QuestionBankRequests.Commands;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.QuestionsBank;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.QuestionBankRequests.Handlers.Commands;

public sealed class SubmitQuestionBankRequestReviewCommandHandler(
    IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    : IRequestHandler<SubmitQuestionBankRequestReviewCommand,
        IResult<SubmitQuestionBankRequestReviewResult>>
{
    private const int ReviewNoteMaxLength = 2000;

    public async Task<IResult<SubmitQuestionBankRequestReviewResult>> Handle(
        SubmitQuestionBankRequestReviewCommand request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(currentUser.UserId, out var reviewerId))
            return Result.Fail<SubmitQuestionBankRequestReviewResult>(ErrorsCodes.InvalidUserIdentifier);
        var validReviewer = await unitOfWork.Context.Set<EmployeeUser>().AsNoTracking().AnyAsync(x =>
            x.Id == reviewerId && !x.IsDeleted && !x.IsBlocked && x.EmployeeProfile != null &&
            !x.EmployeeProfile.IsDeleted, cancellationToken);
        if (!validReviewer)
            return Result.Fail<SubmitQuestionBankRequestReviewResult>(ErrorsCodes.InvalidUserIdentifier);

        if (request.Reviews is null || request.Reviews.Count == 0 ||
            request.Reviews.Select(x => x.RequestItemId).Distinct().Count() != request.Reviews.Count ||
            request.Reviews.Any(InvalidInput))
            return Result.Fail<SubmitQuestionBankRequestReviewResult>(ErrorsCodes.InvalidQuestionBankReview);

        return await unitOfWork.ExecuteInTransactionAsync<IResult<SubmitQuestionBankRequestReviewResult>>(
            async ct =>
            {
                var entity = await unitOfWork.GetEntityRepository<QuestionBankRequest>().DbSet
                    .Include(x => x.Items.Where(i => !i.IsDeleted))
                    .ThenInclude(i => i.QuestionBankAssignment)
                    .Include(x => x.Items.Where(i => !i.IsDeleted))
                    .ThenInclude(i => i.Reviews)
                    .Include(x => x.QuestionBank)
                    .ThenInclude(x => x.Versions)
                    .Include(x => x.BaseVersion)
                    .ThenInclude(x => x!.Questions)
                    .Include(x => x.Assignments.Where(a => !a.IsDeleted))
                    .SingleOrDefaultAsync(x => x.Id == request.RequestId, ct);

                if (entity is null)
                    return Result.Fail<SubmitQuestionBankRequestReviewResult>(ErrorsCodes.QuestionBankRequestNotFound);
                var reviewableItems = entity.Items
                    .Where(i => i.RemovedAt == null &&
                                i.StatusId == QuestionBankRequestItemStatusIds.PENDING_REVIEW)
                    .ToArray();
                if (entity.StatusId != QuestionBankRequestStatusIds.PendingReview || reviewableItems.Length == 0)
                    return Result.Fail<SubmitQuestionBankRequestReviewResult>(ErrorsCodes.QuestionBankRequestNotReviewable);

                var inputs = request.Reviews.ToDictionary(x => x.RequestItemId);
                if (inputs.Count != reviewableItems.Length || reviewableItems.Any(item => !inputs.ContainsKey(item.Id)))
                    return Result.Fail<SubmitQuestionBankRequestReviewResult>(ErrorsCodes.InvalidQuestionBankReview);

                foreach (var item in reviewableItems)
                {
                    var input = inputs[item.Id];
                    var reviewRevisionId = item.ChangeTypeId == QuestionChangeTypeIds.DELETE
                        ? item.OriginalRevisionId
                        : item.CurrentProposedRevisionId;
                    if (!reviewRevisionId.HasValue || reviewRevisionId.Value != input.ReviewedRevisionId)
                        return Result.Fail<SubmitQuestionBankRequestReviewResult>(ErrorsCodes.StaleQuestionBankReview);
                }

                var now = DateTime.UtcNow;
                var round = entity.CurrentReviewRound + 1;
                var changesRequired = false;
                var issued = false;
                var affectedAssignmentIds = new HashSet<Guid>();

                await unitOfWork.GetEntityRepository<QuestionBankRequestReview>().AddAsync(new()
                {
                    Id = Guid.NewGuid(), RequestId = entity.Id, ReviewRound = round,
                    ReviewedById = reviewerId, ReviewedAt = now
                }, ct);

                foreach (var item in reviewableItems)
                {
                    var input = inputs[item.Id];
                    item.StatusId = input.DecisionId switch
                    {
                        var id when id == QuestionReviewDecisionIds.APPROVED =>
                            QuestionBankRequestItemStatusIds.APPROVED,
                        var id when id == QuestionReviewDecisionIds.NEEDS_MODIFICATION =>
                            QuestionBankRequestItemStatusIds.NEEDS_MODIFICATION,
                        _ => QuestionBankRequestItemStatusIds.REJECTED
                    };
                    if (input.DecisionId != QuestionReviewDecisionIds.APPROVED)
                    {
                        changesRequired = true;
                        affectedAssignmentIds.Add(item.QuestionBankAssignmentId);
                    }

                    await unitOfWork.GetEntityRepository<QuestionBankRequestItemReview>().AddAsync(new()
                    {
                        Id = Guid.NewGuid(), RequestItemId = item.Id, ReviewRound = round,
                        ReviewedRevisionId = input.ReviewedRevisionId, DecisionId = input.DecisionId,
                        ReviewNote = string.IsNullOrWhiteSpace(input.ReviewNote) ? null : input.ReviewNote.Trim(),
                        ReviewedById = reviewerId, ReviewedAt = now
                    }, ct);
                }

                entity.CurrentReviewRound = round;
                if (changesRequired)
                {
                    foreach (var assignment in entity.Items
                                 .Where(i => affectedAssignmentIds.Contains(i.QuestionBankAssignmentId))
                                 .Select(i => i.QuestionBankAssignment).DistinctBy(a => a.Id))
                    {
                        assignment.StatusId = QuestionBankAssignmentStatusIds.ReturnedForModification;
                        assignment.LastReturnedForModificationAt = now;
                    }

                    entity.StatusId = QuestionBankRequestStatusIds.ModificationInProgress;
                    await unitOfWork.GetEntityRepository<QuestionBankRequestHistory>().AddAsync(new()
                    {
                        Id = Guid.NewGuid(), RequestId = entity.Id,
                        FromStatusId = QuestionBankRequestStatusIds.PendingReview,
                        ToStatusId = QuestionBankRequestStatusIds.ModificationInProgress,
                        Action = "ReviewChangesRequired", PerformedById = reviewerId, PerformedAt = now
                    }, ct);
                }
                else
                {
                    var includedItems = entity.Items
                        .Where(i => i.RemovedAt == null &&
                                    i.StatusId != QuestionBankRequestItemStatusIds.REMOVED_FROM_REQUEST)
                        .ToArray();
                    var readyToIssue = includedItems.Length > 0 &&
                                       includedItems.All(i =>
                                           i.StatusId == QuestionBankRequestItemStatusIds.APPROVED &&
                                           (i.ChangeTypeId == QuestionChangeTypeIds.DELETE
                                               ? i.OriginalRevisionId.HasValue
                                               : i.CurrentProposedRevisionId.HasValue) &&
                                           (inputs.TryGetValue(i.Id, out var currentReview)
                                               ? currentReview.ReviewedRevisionId ==
                                                 (i.ChangeTypeId == QuestionChangeTypeIds.DELETE
                                                     ? i.OriginalRevisionId!.Value
                                                     : i.CurrentProposedRevisionId!.Value)
                                               : i.Reviews.OrderByDescending(r => r.ReviewRound)
                                                   .ThenByDescending(r => r.ReviewedAt)
                                                   .Select(r => new { r.DecisionId, r.ReviewedRevisionId })
                                                   .FirstOrDefault() is { } lastReview &&
                                                 lastReview.DecisionId == QuestionReviewDecisionIds.APPROVED &&
                                                 lastReview.ReviewedRevisionId ==
                                                 (i.ChangeTypeId == QuestionChangeTypeIds.DELETE
                                                     ? i.OriginalRevisionId!.Value
                                                     : i.CurrentProposedRevisionId!.Value)));

                    // An all-approved payload must either issue the complete request or persist nothing.
                    // This prevents a corrupt/stale request from being left in PendingReview with no
                    // reviewable items when an older active item is unresolved or its revision changed.
                    if (!readyToIssue)
                        return Result.Fail<SubmitQuestionBankRequestReviewResult>(
                            ErrorsCodes.InvalidQuestionBankReview);

                    var maintenance = entity.RequestTypeId == QuestionBankRequestTypeIds.MAINTENANCE;
                    if (maintenance && (entity.BaseVersionId is null || entity.BaseVersion is null ||
                                        entity.QuestionBank.CurrentApprovedVersionId != entity.BaseVersionId))
                        return Result.Fail<SubmitQuestionBankRequestReviewResult>(ErrorsCodes.QuestionBankBaseVersionStale);
                    if (maintenance)
                    {
                        var baseRevisions = entity.BaseVersion!.Questions.Where(x => !x.IsDeleted)
                            .ToDictionary(x => x.QuestionId, x => x.QuestionRevisionId);
                        var invalidChange = includedItems.Any(item => item.ChangeTypeId switch
                        {
                            var type when type == QuestionChangeTypeIds.ADD => item.OriginalRevisionId.HasValue,
                            var type when type == QuestionChangeTypeIds.UPDATE || type == QuestionChangeTypeIds.DELETE =>
                                !baseRevisions.TryGetValue(item.QuestionId, out var original) ||
                                item.OriginalRevisionId != original,
                            _ => true
                        });
                        if (invalidChange)
                            return Result.Fail<SubmitQuestionBankRequestReviewResult>(ErrorsCodes.InvalidQuestionBankMaintenanceChange);
                    }

                    var versionId = Guid.NewGuid();
                    var versionNo = entity.QuestionBank.Versions.Count == 0
                        ? 1
                        : entity.QuestionBank.Versions.Max(v => v.VersionNo) + 1;
                    var version = new QuestionBankVersion
                    {
                        Id = versionId,
                        QuestionBankId = entity.QuestionBankId,
                        VersionNo = versionNo,
                        PreviousVersionId = maintenance ? entity.BaseVersionId : entity.QuestionBank.CurrentApprovedVersionId,
                        CreatedFromRequestId = entity.Id,
                        ApprovedById = reviewerId,
                        ApprovedAt = now,
                        EffectiveFrom = now
                    };
                    if (maintenance)
                    {
                        var changes = includedItems.ToDictionary(x => x.QuestionId);
                        foreach (var baseQuestion in entity.BaseVersion!.Questions.Where(x => !x.IsDeleted))
                        {
                            if (changes.TryGetValue(baseQuestion.QuestionId, out var change) &&
                                change.ChangeTypeId == QuestionChangeTypeIds.DELETE) continue;
                            version.Questions.Add(new QuestionBankVersionQuestion
                            {
                                Id = Guid.NewGuid(), QuestionBankVersionId = versionId,
                                QuestionId = baseQuestion.QuestionId,
                                QuestionRevisionId = change?.CurrentProposedRevisionId ?? baseQuestion.QuestionRevisionId,
                                SourceRequestItemId = change?.Id
                            });
                        }
                        foreach (var addition in includedItems.Where(x => x.ChangeTypeId == QuestionChangeTypeIds.ADD))
                            version.Questions.Add(new QuestionBankVersionQuestion
                            {
                                Id = Guid.NewGuid(), QuestionBankVersionId = versionId,
                                QuestionId = addition.QuestionId,
                                QuestionRevisionId = addition.CurrentProposedRevisionId!.Value,
                                SourceRequestItemId = addition.Id
                            });
                    }
                    else
                    {
                        foreach (var item in includedItems)
                            version.Questions.Add(new QuestionBankVersionQuestion
                            {
                                Id = Guid.NewGuid(), QuestionBankVersionId = versionId,
                                QuestionId = item.QuestionId,
                                QuestionRevisionId = item.CurrentProposedRevisionId!.Value,
                                SourceRequestItemId = item.Id
                            });
                    }

                    await unitOfWork.GetEntityRepository<QuestionBankVersion>().AddAsync(version, ct);
                    entity.QuestionBank.CurrentApprovedVersionId = versionId;
                    entity.QuestionBank.IsActive = true;
                    foreach (var assignment in entity.Assignments)
                    {
                        assignment.StatusId = QuestionBankAssignmentStatusIds.Completed;
                        assignment.CompletedAt = now;
                    }

                    entity.StatusId = QuestionBankRequestStatusIds.Issued;
                    issued = true;
                    entity.FinalDecisionById = reviewerId;
                    entity.FinalDecisionAt = now;
                    await unitOfWork.GetEntityRepository<QuestionBankRequestHistory>().AddAsync(new()
                    {
                        Id = Guid.NewGuid(), RequestId = entity.Id,
                        FromStatusId = QuestionBankRequestStatusIds.PendingReview,
                        ToStatusId = QuestionBankRequestStatusIds.Issued,
                        Action = "ReviewApprovedAndIssued", PerformedById = reviewerId, PerformedAt = now
                    }, ct);
                }

                try
                {
                    await unitOfWork.SaveChangesAsync(ct);
                }
                catch (DbUpdateConcurrencyException) when (entity.RequestTypeId == QuestionBankRequestTypeIds.MAINTENANCE)
                {
                    return Result.Fail<SubmitQuestionBankRequestReviewResult>(ErrorsCodes.QuestionBankBaseVersionStale);
                }
                return Result.Ok(new SubmitQuestionBankRequestReviewResult(changesRequired, issued, round));
            }, cancellationToken);
    }

    private static bool InvalidInput(QuestionReviewInput input)
    {
        var validDecision = input.DecisionId == QuestionReviewDecisionIds.APPROVED ||
                            input.DecisionId == QuestionReviewDecisionIds.NEEDS_MODIFICATION ||
                            input.DecisionId == QuestionReviewDecisionIds.REJECTED;
        var noteRequired = input.DecisionId != QuestionReviewDecisionIds.APPROVED;
        return input.RequestItemId == Guid.Empty || input.ReviewedRevisionId == Guid.Empty ||
               !validDecision || input.ReviewNote?.Length > ReviewNoteMaxLength ||
               (noteRequired && string.IsNullOrWhiteSpace(input.ReviewNote));
    }
}
