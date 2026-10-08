using Application.Operation.Features.Employee.Exams.DTOs;
using Application.Operation.Features.Employee.Exams.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.QuestionsBank;

namespace Application.Operation.Features.Employee.Exams.Handlers.Queries;

public sealed class GetExamResultCandidateAttemptDetailsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetExamResultCandidateAttemptDetailsQuery, IResult<ExamResultCandidateAttemptDetailsDto>>
{
    public async Task<IResult<ExamResultCandidateAttemptDetailsDto>> Handle(
        GetExamResultCandidateAttemptDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var candidate = await unitOfWork.Context.Set<ExamResultCandidate>()
            .AsNoTracking()
            .Where(x => x.Id == request.CandidateResultId && x.ExamResultReportId == request.ReportId)
            .Select(x => new
            {
                x.Id,
                x.InvitationId,
                x.TestSessionCandidateId,
                x.TestAttemptId,
                x.SpecializedScore,
                x.EducationalScore,
                x.SkillsScore,
                x.FinalScore,
                x.Rank,
                x.SnapshotAt,
                ResultStatus = new ExamResultLookupDto(x.ResultStatusId,
                    x.ResultStatus!.BackendName, x.ResultStatus.NameAr, x.ResultStatus.NameEn),
                AttemptStatus = x.TestAttempt == null ? null : new ExamResultLookupDto(
                    x.TestAttempt.StatusId, x.TestAttempt.Status!.BackendName,
                    x.TestAttempt.Status.NameAr, x.TestAttempt.Status.NameEn),
                StartedAt = x.TestAttempt == null ? null : x.TestAttempt.StartedAt,
                CompletedAt = x.TestAttempt == null ? null : x.TestAttempt.CompletedAt
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (candidate is null)
            return Result.Fail<ExamResultCandidateAttemptDetailsDto>(
                new Error(ErrorsCodes.ExamResultCandidateNotFound));

        var parts = new List<ExamResultAttemptPartDto>();
        if (candidate.TestAttemptId is Guid attemptId)
        {
            var partRows = await unitOfWork.Context.Set<TestAttemptPart>()
                .AsNoTracking()
                .Where(x => x.TestAttemptId == attemptId)
                .OrderBy(x => x.ExamPart!.PartNo)
                .Select(x => new
                {
                    x.Id,
                    x.ExamPart!.PartNo,
                    x.StartedAt,
                    x.CompletedAt,
                    x.DurationMinutesSnapshot,
                    x.EffectiveDeadlineAt,
                    Status = new ExamResultLookupDto(x.StatusId,
                        x.Status!.BackendName, x.Status.NameAr, x.Status.NameEn)
                })
                .ToListAsync(cancellationToken);

            var questionRows = await unitOfWork.Context.Set<TestAttemptQuestion>()
                .AsNoTracking()
                .Where(x => x.TestAttemptId == attemptId)
                .OrderBy(x => x.TestAttemptPart!.ExamPart!.PartNo)
                .ThenBy(x => x.OrderNo)
                .Select(x => new
                {
                    x.Id,
                    x.TestAttemptPartId,
                    x.OrderNo,
                    x.QuestionRevisionId,
                    x.QuestionRevision!.QuestionTextAr,
                    x.QuestionRevision.QuestionTextEn,
                    QuestionType = new ExamResultLookupDto(x.QuestionRevision.QuestionTypeId,
                        x.QuestionRevision.QuestionType.BackendName,
                        x.QuestionRevision.QuestionType.NameAr,
                        x.QuestionRevision.QuestionType.NameEn),
                    Category = new ExamResultLookupDto(x.ExamCategory!.QuestionBankTypeId,
                        x.ExamCategory.QuestionBankType!.BackendName,
                        x.ExamCategory.QuestionBankType.NameAr,
                        x.ExamCategory.QuestionBankType.NameEn),
                    x.SelectedQuestionRevisionOptionId,
                    x.SavedAt
                })
                .ToListAsync(cancellationToken);

            var revisionIds = questionRows.Select(x => x.QuestionRevisionId).Distinct().ToList();
            var optionRows = await unitOfWork.Context.Set<QuestionRevisionOption>()
                .AsNoTracking()
                .Where(x => revisionIds.Contains(x.QuestionRevisionId))
                .OrderBy(x => x.DisplayOrder)
                .Select(x => new
                {
                    x.Id,
                    x.QuestionRevisionId,
                    x.OptionTextAr,
                    x.OptionTextEn,
                    x.DisplayOrder,
                    x.IsCorrect
                })
                .ToListAsync(cancellationToken);

            var optionsByRevision = optionRows
                .GroupBy(x => x.QuestionRevisionId)
                .ToDictionary(x => x.Key, x => x.ToList());
            var questionsByPart = questionRows
                .GroupBy(x => x.TestAttemptPartId)
                .ToDictionary(x => x.Key, x => x.Select(question =>
                {
                    var revisionOptions = optionsByRevision.GetValueOrDefault(
                        question.QuestionRevisionId, []);
                    var selected = revisionOptions.FirstOrDefault(option =>
                        option.Id == question.SelectedQuestionRevisionOptionId);
                    return new ExamResultAttemptQuestionDto(
                        question.Id,
                        question.OrderNo,
                        question.QuestionRevisionId,
                        question.QuestionTextAr,
                        question.QuestionTextEn,
                        question.QuestionType,
                        question.Category,
                        question.SelectedQuestionRevisionOptionId,
                        question.SavedAt.AsUtcOffset(),
                        question.SelectedQuestionRevisionOptionId.HasValue,
                        selected?.IsCorrect,
                        revisionOptions.Select(option => new ExamResultAttemptOptionDto(
                            option.Id,
                            option.OptionTextAr,
                            option.OptionTextEn,
                            option.DisplayOrder,
                            option.Id == question.SelectedQuestionRevisionOptionId)).ToList());
                }).ToList());

            parts = partRows.Select(part => new ExamResultAttemptPartDto(
                part.Id,
                part.PartNo,
                part.Status,
                part.StartedAt.AsUtcOffset(),
                part.CompletedAt.AsUtcOffset(),
                part.DurationMinutesSnapshot,
                part.EffectiveDeadlineAt.AsUtcOffset(),
                questionsByPart.GetValueOrDefault(part.Id, []))).ToList();
        }

        return Result.Ok(new ExamResultCandidateAttemptDetailsDto(
            candidate.Id,
            candidate.InvitationId,
            candidate.TestSessionCandidateId,
            candidate.TestAttemptId,
            candidate.ResultStatus,
            candidate.SpecializedScore,
            candidate.EducationalScore,
            candidate.SkillsScore,
            candidate.FinalScore,
            candidate.Rank,
            candidate.SnapshotAt.AsUtcOffset(),
            candidate.TestAttemptId.HasValue,
            candidate.AttemptStatus,
            candidate.StartedAt.AsUtcOffset(),
            candidate.CompletedAt.AsUtcOffset(),
            parts));
    }
}
