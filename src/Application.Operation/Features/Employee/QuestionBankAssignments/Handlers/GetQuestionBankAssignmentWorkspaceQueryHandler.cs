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

public sealed class GetQuestionBankAssignmentWorkspaceQueryHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    : IRequestHandler<GetQuestionBankAssignmentWorkspaceQuery, IResult<QuestionBankAssignmentWorkspaceDto>>
{
    public async Task<IResult<QuestionBankAssignmentWorkspaceDto>> Handle(GetQuestionBankAssignmentWorkspaceQuery r, CancellationToken ct)
    {
        var employeeId = await AssignmentIdentity.CurrentEmployeeId(uow, currentUser, ct);
        if (employeeId is null) return Result.Fail<QuestionBankAssignmentWorkspaceDto>(ErrorsCodes.InvalidUserIdentifier);
        var a = await uow.GetEntityRepository<QuestionBankAssignment>().DbSet.AsNoTracking()
            .Include(x => x.Status).Include(x => x.QuestionBankRequest).ThenInclude(x => x.QuestionBank).ThenInclude(x => x.QuestionBankType)
            .Include(x => x.QuestionBankRequest.QuestionBank.Management).Include(x => x.QuestionBankRequest.QuestionBank.JobTitle)
            .SingleOrDefaultAsync(x => x.Id == r.AssignmentId && x.EmployeeId == employeeId && !x.IsDeleted, ct);
        if (a is null) return Result.Fail<QuestionBankAssignmentWorkspaceDto>(ErrorsCodes.QuestionBankAssignmentNotFound);
        var questions = await uow.GetEntityRepository<QuestionBankRequestItem>().DbSet.AsNoTracking()
            .Where(x => x.QuestionBankAssignmentId == a.Id && x.RequestId == a.QuestionBankRequestId && !x.IsDeleted &&
                        x.StatusId != QuestionBankRequestItemStatusIds.REMOVED_FROM_REQUEST)
            .OrderBy(x => x.CreatedDate).Select(x => new AssignmentQuestionDto(x.Id, x.QuestionId, x.ChangeTypeId,
                x.CurrentProposedRevisionId ?? x.OriginalRevisionId!.Value,
                x.CurrentProposedRevision != null ? x.CurrentProposedRevision.RevisionNo : x.OriginalRevision!.RevisionNo,
                x.CurrentProposedRevision != null ? x.CurrentProposedRevision.QuestionTypeId : x.OriginalRevision!.QuestionTypeId,
                x.CurrentProposedRevision != null ? x.CurrentProposedRevision.QuestionType.NameAr : x.OriginalRevision!.QuestionType.NameAr,
                x.CurrentProposedRevision != null ? x.CurrentProposedRevision.QuestionType.NameEn : x.OriginalRevision!.QuestionType.NameEn,
                x.CurrentProposedRevision != null ? x.CurrentProposedRevision.DifficultyLevelId : x.OriginalRevision!.DifficultyLevelId,
                x.CurrentProposedRevision != null ? x.CurrentProposedRevision.DifficultyLevel.NameAr : x.OriginalRevision!.DifficultyLevel.NameAr,
                x.CurrentProposedRevision != null ? x.CurrentProposedRevision.DifficultyLevel.NameEn : x.OriginalRevision!.DifficultyLevel.NameEn,
                x.CurrentProposedRevision != null ? x.CurrentProposedRevision.QuestionTextAr : x.OriginalRevision!.QuestionTextAr,
                x.CurrentProposedRevision != null ? x.CurrentProposedRevision.QuestionTextEn : x.OriginalRevision!.QuestionTextEn,
                x.CurrentProposedRevision != null ? x.CurrentProposedRevision.ExplanationAr : x.OriginalRevision!.ExplanationAr,
                x.CurrentProposedRevision != null ? x.CurrentProposedRevision.ExplanationEn : x.OriginalRevision!.ExplanationEn,
                x.CurrentProposedRevision != null ? x.CurrentProposedRevision.ResourceId : x.OriginalRevision!.ResourceId,
                x.CurrentProposedRevision != null
                    ? (x.CurrentProposedRevision.ResourceId == null ? null : x.CurrentProposedRevision.Resource!.Url)
                    : (x.OriginalRevision!.ResourceId == null ? null : x.OriginalRevision.Resource!.Url),
                x.StatusId, x.Status.NameAr, x.Status.NameEn,
                x.CurrentProposedRevision != null
                    ? x.CurrentProposedRevision.Options.OrderBy(o => o.DisplayOrder)
                        .Select(o => new QuestionOptionDto(o.Id, o.OptionTextAr, o.OptionTextEn, o.IsCorrect, o.DisplayOrder)).ToList()
                    : x.OriginalRevision!.Options.OrderBy(o => o.DisplayOrder)
                        .Select(o => new QuestionOptionDto(o.Id, o.OptionTextAr, o.OptionTextEn, o.IsCorrect, o.DisplayOrder)).ToList(),
                x.Reviews.OrderByDescending(review => review.ReviewRound).Select(review => (Guid?)review.DecisionId).FirstOrDefault(),
                x.Reviews.OrderByDescending(review => review.ReviewRound).Select(review => review.ReviewNote).FirstOrDefault(),
                x.Reviews.OrderByDescending(review => review.ReviewRound).Select(review => (int?)review.ReviewRound).FirstOrDefault())).ToListAsync(ct);
        var baseQuestions = a.QuestionBankRequest.RequestTypeId == QuestionBankRequestTypeIds.MAINTENANCE &&
                            a.QuestionBankRequest.BaseVersionId.HasValue
            ? await uow.GetEntityRepository<QuestionBankVersionQuestion>().DbSet.AsNoTracking()
                .Where(x => x.QuestionBankVersionId == a.QuestionBankRequest.BaseVersionId && !x.IsDeleted)
                .OrderBy(x => x.CreatedDate)
                .Select(x => new MaintenanceBaseQuestionDto(x.QuestionId, x.QuestionRevisionId,
                    x.QuestionRevision.RevisionNo, x.QuestionRevision.QuestionTypeId,
                    x.QuestionRevision.QuestionType.NameAr, x.QuestionRevision.QuestionType.NameEn,
                    x.QuestionRevision.DifficultyLevelId, x.QuestionRevision.DifficultyLevel.NameAr,
                    x.QuestionRevision.DifficultyLevel.NameEn, x.QuestionRevision.QuestionTextAr,
                    x.QuestionRevision.QuestionTextEn, x.QuestionRevision.ExplanationAr,
                    x.QuestionRevision.ExplanationEn, x.QuestionRevision.ResourceId,
                    x.QuestionRevision.ResourceId == null ? null : x.QuestionRevision.Resource!.Url,
                    x.QuestionRevision.Options.OrderBy(o => o.DisplayOrder)
                        .Select(o => new QuestionOptionDto(o.Id, o.OptionTextAr, o.OptionTextEn, o.IsCorrect, o.DisplayOrder)).ToList(),
                    x.Question.RequestItems.Any(i => !i.IsDeleted && i.RequestId == a.QuestionBankRequestId)
                        ? (x.Question.RequestItems.Any(i => !i.IsDeleted && i.RequestId == a.QuestionBankRequestId &&
                                                           i.QuestionBankAssignmentId == a.Id)
                            ? "AssignedToMe" : "AssignedToAnotherEmployee")
                        : "Available",
                    x.Question.RequestItems.Where(i => !i.IsDeleted && i.RequestId == a.QuestionBankRequestId)
                        .Select(i => (Guid?)i.Id).FirstOrDefault())).ToListAsync(ct)
            : [];
        var current = questions.Count(x => x.StatusId != QuestionBankRequestItemStatusIds.REJECTED); var bank = a.QuestionBankRequest.QuestionBank;
        return Result.Ok(new QuestionBankAssignmentWorkspaceDto(
            new(a.Id, a.StatusId, a.Status.NameAr, a.Status.NameEn, a.MinimumQuestionCount, a.Notes, a.AssignedAt, a.QuestionEntryStartedAt, a.QuestionEntryCompletedAt),
            new(a.QuestionBankRequestId, a.QuestionBankRequest.StatusId),
            new(bank.QuestionBankTypeId, bank.QuestionBankType.NameAr, bank.QuestionBankType.NameEn,
                bank.Management?.NameAr, bank.Management?.NameEn, bank.JobTitle?.JobNameAr, bank.JobTitle?.JobNameEn),
            new(a.MinimumQuestionCount, current, Math.Max(0, a.MinimumQuestionCount-current),
                current >= a.MinimumQuestionCount && current > 0 &&
                questions.All(x => x.StatusId != QuestionBankRequestItemStatusIds.NEEDS_MODIFICATION &&
                                   x.StatusId != QuestionBankRequestItemStatusIds.REJECTED)), questions, baseQuestions));
    }
}
