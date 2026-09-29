using Application.Operation.Features.Employee.QuestionBankRequests.Commands;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.QuestionsBank;

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

        if (request.Reviews is null || request.Reviews.Count == 0 ||
            request.Reviews.Select(x => x.RequestItemId).Distinct().Count() != request.Reviews.Count ||
            request.Reviews.Any(InvalidInput))
            return Result.Fail<SubmitQuestionBankRequestReviewResult>(ErrorsCodes.InvalidQuestionBankReview);

        return await unitOfWork.ExecuteInTransactionAsync<IResult<SubmitQuestionBankRequestReviewResult>>(
            async ct =>
            {
                var entity = await unitOfWork.GetEntityRepository<QuestionBankRequest>().DbSet
                    .Include(x => x.Items.Where(i => !i.IsDeleted && i.RemovedAt == null &&
                        i.StatusId == QuestionBankRequestItemStatusIds.PENDING_REVIEW))
                    .ThenInclude(i => i.QuestionBankAssignment)
                    .SingleOrDefaultAsync(x => x.Id == request.RequestId, ct);

                if (entity is null)
                    return Result.Fail<SubmitQuestionBankRequestReviewResult>(ErrorsCodes.QuestionBankRequestNotFound);
                if (entity.StatusId != QuestionBankRequestStatusIds.PendingReview || entity.Items.Count == 0)
                    return Result.Fail<SubmitQuestionBankRequestReviewResult>(ErrorsCodes.QuestionBankRequestNotReviewable);

                var inputs = request.Reviews.ToDictionary(x => x.RequestItemId);
                if (inputs.Count != entity.Items.Count || entity.Items.Any(item => !inputs.ContainsKey(item.Id)))
                    return Result.Fail<SubmitQuestionBankRequestReviewResult>(ErrorsCodes.InvalidQuestionBankReview);

                foreach (var item in entity.Items)
                {
                    var input = inputs[item.Id];
                    if (!item.CurrentProposedRevisionId.HasValue ||
                        item.CurrentProposedRevisionId.Value != input.ReviewedRevisionId)
                        return Result.Fail<SubmitQuestionBankRequestReviewResult>(ErrorsCodes.StaleQuestionBankReview);
                }

                var now = DateTime.UtcNow;
                var round = entity.CurrentReviewRound + 1;
                var changesRequired = false;
                var affectedAssignmentIds = new HashSet<Guid>();

                await unitOfWork.GetEntityRepository<QuestionBankRequestReview>().AddAsync(new()
                {
                    Id = Guid.NewGuid(), RequestId = entity.Id, ReviewRound = round,
                    ReviewedById = reviewerId, ReviewedAt = now
                }, ct);

                foreach (var item in entity.Items)
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

                await unitOfWork.SaveChangesAsync(ct);
                return Result.Ok(new SubmitQuestionBankRequestReviewResult(changesRequired, round));
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
