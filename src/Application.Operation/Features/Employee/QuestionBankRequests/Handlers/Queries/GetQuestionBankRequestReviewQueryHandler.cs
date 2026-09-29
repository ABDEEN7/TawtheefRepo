using Application.Operation.Features.Employee.QuestionBankRequests.DTOs;
using Application.Operation.Features.Employee.QuestionBankRequests.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.QuestionsBank;

namespace Application.Operation.Features.Employee.QuestionBankRequests.Handlers.Queries;

public sealed class GetQuestionBankRequestReviewQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetQuestionBankRequestReviewQuery, IResult<QuestionBankRequestReviewDto>>
{
    public async Task<IResult<QuestionBankRequestReviewDto>> Handle(
        GetQuestionBankRequestReviewQuery request, CancellationToken cancellationToken)
    {
        var review = await unitOfWork.GetEntityRepository<QuestionBankRequest>().DbSet
            .AsNoTracking()
            .Where(x => x.Id == request.RequestId)
            .Select(x => new QuestionBankRequestReviewDto
            {
                RequestId = x.Id,
                QuestionBankId = x.QuestionBankId,
                RequestTypeId = x.RequestTypeId,
                RequestTypeNameAr = x.RequestType.NameAr,
                RequestTypeNameEn = x.RequestType.NameEn,
                RequestStatusId = x.StatusId,
                RequestStatusNameAr = x.Status.NameAr,
                RequestStatusNameEn = x.Status.NameEn,
                CurrentReviewRound = x.CurrentReviewRound,
                QuestionBankTypeId = x.QuestionBank.QuestionBankTypeId,
                QuestionBankTypeNameAr = x.QuestionBank.QuestionBankType.NameAr,
                QuestionBankTypeNameEn = x.QuestionBank.QuestionBankType.NameEn,
                ManagementNameAr = x.QuestionBank.Management == null
                    ? null
                    : x.QuestionBank.Management.NameAr,
                ManagementNameEn = x.QuestionBank.Management == null
                    ? null
                    : x.QuestionBank.Management.NameEn,
                JobTitleNameAr = x.QuestionBank.JobTitle == null
                    ? null
                    : x.QuestionBank.JobTitle.JobNameAr,
                JobTitleNameEn = x.QuestionBank.JobTitle == null
                    ? null
                    : x.QuestionBank.JobTitle.JobNameEn,
                Items = x.Items
                    .Where(i => !i.IsDeleted && i.RemovedAt == null &&
                                i.StatusId == QuestionBankRequestItemStatusIds.PENDING_REVIEW &&
                                i.CurrentProposedRevisionId != null)
                    .OrderBy(i => i.CreatedDate)
                    .Select(i => new QuestionBankReviewItemDto
                    {
                        RequestItemId = i.Id,
                        QuestionId = i.QuestionId,
                        QuestionBankAssignmentId = i.QuestionBankAssignmentId,
                        EmployeeNameAr = i.QuestionBankAssignment.Employee.FullNameAr,
                        EmployeeNameEn = i.QuestionBankAssignment.Employee.FullNameEn,
                        ChangeTypeId = i.ChangeTypeId,
                        ItemStatusId = i.StatusId,
                        CurrentProposedRevisionId = i.CurrentProposedRevisionId!.Value,
                        QuestionTypeId = i.CurrentProposedRevision!.QuestionTypeId,
                        QuestionTypeNameAr = i.CurrentProposedRevision.QuestionType.NameAr,
                        QuestionTypeNameEn = i.CurrentProposedRevision.QuestionType.NameEn,
                        DifficultyLevelId = i.CurrentProposedRevision.DifficultyLevelId,
                        DifficultyNameAr = i.CurrentProposedRevision.DifficultyLevel.NameAr,
                        DifficultyNameEn = i.CurrentProposedRevision.DifficultyLevel.NameEn,
                        QuestionTextAr = i.CurrentProposedRevision.QuestionTextAr,
                        QuestionTextEn = i.CurrentProposedRevision.QuestionTextEn,
                        ExplanationAr = i.CurrentProposedRevision.ExplanationAr,
                        ExplanationEn = i.CurrentProposedRevision.ExplanationEn,
                        ImageUrl = i.CurrentProposedRevision.ResourceId == null
                            ? null
                            : i.CurrentProposedRevision.Resource!.Url,
                        Options = i.CurrentProposedRevision.Options
                            .OrderBy(o => o.DisplayOrder)
                            .Select(o => new QuestionBankReviewOptionDto
                            {
                                Id = o.Id,
                                OptionTextAr = o.OptionTextAr,
                                OptionTextEn = o.OptionTextEn,
                                IsCorrect = o.IsCorrect,
                                DisplayOrder = o.DisplayOrder
                            })
                            .ToList()
                    })
                    .ToList()
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (review is null)
            return Result.Fail<QuestionBankRequestReviewDto>(ErrorsCodes.QuestionBankRequestNotFound);
        if (review.RequestStatusId != QuestionBankRequestStatusIds.PendingReview || review.Items.Count == 0)
            return Result.Fail<QuestionBankRequestReviewDto>(ErrorsCodes.QuestionBankRequestNotReviewable);
        return Result.Ok(review);
    }
}
