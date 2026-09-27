using Application.Recruitment.Features.CandidateExams.Models;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Exams;

namespace Application.Recruitment.Features.CandidateExams.Services;

public sealed class CandidateExamScoringService(IUnitOfWork unitOfWork)
{
    public async Task<decimal?> CalculatePartScoreAsync(
        Guid attemptId, Guid partId, CancellationToken cancellationToken)
    {
        var scores = await CalculatePartCategoryScoresAsync(attemptId, partId, cancellationToken);
        return scores is { Count: 1 } ? scores[0].Score : null;
    }

    public async Task<IReadOnlyList<CandidateExamCategoryScore>?> CalculatePartCategoryScoresAsync(
        Guid attemptId, Guid partId, CancellationToken cancellationToken)
    {
        var examPartId = await unitOfWork.Context.Set<TestAttemptPart>()
            .AsNoTracking()
            .Where(part => part.Id == partId && part.TestAttemptId == attemptId)
            .Select(part => (Guid?)part.ExamPartId)
            .SingleOrDefaultAsync(cancellationToken);
        if (examPartId is null)
            return null;

        var categories = await unitOfWork.Context.Set<ExamCategory>()
            .AsNoTracking()
            .Where(category => category.ExamPartId == examPartId.Value)
            .Select(category => new
            {
                category.Id, category.QuestionBankTypeId, category.WeightPercent, category.QuestionCount
            })
            .ToListAsync(cancellationToken);

        if (categories.Count == 0 || categories.Any(category => category.QuestionCount <= 0) ||
            categories.Select(category => category.QuestionBankTypeId).Distinct().Count() != categories.Count)
            return null;

        var answers = await unitOfWork.Context.Set<TestAttemptQuestion>()
            .AsNoTracking()
            .Where(question => question.TestAttemptId == attemptId && question.TestAttemptPartId == partId)
            .Select(question => new
            {
                question.ExamCategoryId,
                IsCorrect = question.SelectedQuestionRevisionOption != null &&
                            question.SelectedQuestionRevisionOption.QuestionRevisionId == question.QuestionRevisionId &&
                            question.SelectedQuestionRevisionOption.IsCorrect
            })
            .ToListAsync(cancellationToken);

        if (categories.Any(category =>
                answers.Count(answer => answer.ExamCategoryId == category.Id) != category.QuestionCount) ||
            answers.Any(answer => categories.All(category => category.Id != answer.ExamCategoryId)))
            return null;

        return
        [
            .. categories.Select(category => new CandidateExamCategoryScore(
                category.QuestionBankTypeId,
                category.WeightPercent,
                decimal.Round(
                    100m *
                    answers.Count(answer => answer.ExamCategoryId == category.Id && answer.IsCorrect) /
                    category.QuestionCount,
                    2,
                    MidpointRounding.AwayFromZero)))
        ];
    }
}
