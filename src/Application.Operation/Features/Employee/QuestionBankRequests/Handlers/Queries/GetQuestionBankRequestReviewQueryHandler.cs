using Application.Operation.Features.Employee.QuestionBankRequests.DTOs;
using Application.Operation.Features.Employee.QuestionBankRequests.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.QuestionsBank;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.QuestionBankRequests.Handlers.Queries;

public sealed class GetQuestionBankRequestReviewQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    : IRequestHandler<GetQuestionBankRequestReviewQuery, IResult<QuestionBankRequestReviewDto>>
{
    public async Task<IResult<QuestionBankRequestReviewDto>> Handle(
        GetQuestionBankRequestReviewQuery request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(currentUser.UserId, out var reviewerId) ||
            !await unitOfWork.Context.Set<EmployeeUser>().AsNoTracking().AnyAsync(x =>
                x.Id == reviewerId && !x.IsDeleted && !x.IsBlocked && x.EmployeeProfile != null &&
                !x.EmployeeProfile.IsDeleted, cancellationToken))
            return Result.Fail<QuestionBankRequestReviewDto>(ErrorsCodes.InvalidUserIdentifier);

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
                                (i.CurrentProposedRevisionId != null ||
                                 (i.ChangeTypeId == QuestionChangeTypeIds.DELETE && i.OriginalRevisionId != null)))
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
                        CurrentProposedRevisionId = i.CurrentProposedRevisionId ?? i.OriginalRevisionId!.Value,
                        OriginalRevisionId = i.OriginalRevisionId,
                        OriginalQuestionTextAr = i.OriginalRevision == null ? null : i.OriginalRevision.QuestionTextAr,
                        OriginalQuestionTextEn = i.OriginalRevision == null ? null : i.OriginalRevision.QuestionTextEn,
                        OriginalDifficultyNameAr = i.OriginalRevision == null ? null : i.OriginalRevision.DifficultyLevel.NameAr,
                        OriginalDifficultyNameEn = i.OriginalRevision == null ? null : i.OriginalRevision.DifficultyLevel.NameEn,
                        OriginalImageUrl = i.OriginalRevision == null || i.OriginalRevision.ResourceId == null
                            ? null : i.OriginalRevision.Resource!.Url,
                        OriginalOptions = i.OriginalRevision == null ? new List<QuestionBankReviewOptionDto>() :
                            i.OriginalRevision.Options.OrderBy(o => o.DisplayOrder).Select(o => new QuestionBankReviewOptionDto
                            {
                                Id = o.Id, OptionTextAr = o.OptionTextAr, OptionTextEn = o.OptionTextEn,
                                IsCorrect = o.IsCorrect, DisplayOrder = o.DisplayOrder
                            }).ToList(),
                        QuestionTypeId = i.CurrentProposedRevision != null ? i.CurrentProposedRevision.QuestionTypeId : i.OriginalRevision!.QuestionTypeId,
                        QuestionTypeNameAr = i.CurrentProposedRevision != null ? i.CurrentProposedRevision.QuestionType.NameAr : i.OriginalRevision!.QuestionType.NameAr,
                        QuestionTypeNameEn = i.CurrentProposedRevision != null ? i.CurrentProposedRevision.QuestionType.NameEn : i.OriginalRevision!.QuestionType.NameEn,
                        DifficultyLevelId = i.CurrentProposedRevision != null ? i.CurrentProposedRevision.DifficultyLevelId : i.OriginalRevision!.DifficultyLevelId,
                        DifficultyNameAr = i.CurrentProposedRevision != null ? i.CurrentProposedRevision.DifficultyLevel.NameAr : i.OriginalRevision!.DifficultyLevel.NameAr,
                        DifficultyNameEn = i.CurrentProposedRevision != null ? i.CurrentProposedRevision.DifficultyLevel.NameEn : i.OriginalRevision!.DifficultyLevel.NameEn,
                        QuestionTextAr = i.CurrentProposedRevision != null ? i.CurrentProposedRevision.QuestionTextAr : i.OriginalRevision!.QuestionTextAr,
                        QuestionTextEn = i.CurrentProposedRevision != null ? i.CurrentProposedRevision.QuestionTextEn : i.OriginalRevision!.QuestionTextEn,
                        ExplanationAr = i.CurrentProposedRevision != null ? i.CurrentProposedRevision.ExplanationAr : i.OriginalRevision!.ExplanationAr,
                        ExplanationEn = i.CurrentProposedRevision != null ? i.CurrentProposedRevision.ExplanationEn : i.OriginalRevision!.ExplanationEn,
                        ImageUrl = i.CurrentProposedRevision != null
                            ? (i.CurrentProposedRevision.ResourceId == null ? null : i.CurrentProposedRevision.Resource!.Url)
                            : (i.OriginalRevision!.ResourceId == null ? null : i.OriginalRevision.Resource!.Url),
                        Options = i.CurrentProposedRevision != null
                            ? i.CurrentProposedRevision.Options.OrderBy(o => o.DisplayOrder)
                                .Select(o => new QuestionBankReviewOptionDto
                                {
                                    Id = o.Id, OptionTextAr = o.OptionTextAr, OptionTextEn = o.OptionTextEn,
                                    IsCorrect = o.IsCorrect, DisplayOrder = o.DisplayOrder
                                }).ToList()
                            : i.OriginalRevision!.Options.OrderBy(o => o.DisplayOrder)
                                .Select(o => new QuestionBankReviewOptionDto
                                {
                                    Id = o.Id, OptionTextAr = o.OptionTextAr, OptionTextEn = o.OptionTextEn,
                                    IsCorrect = o.IsCorrect, DisplayOrder = o.DisplayOrder
                                }).ToList()
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
