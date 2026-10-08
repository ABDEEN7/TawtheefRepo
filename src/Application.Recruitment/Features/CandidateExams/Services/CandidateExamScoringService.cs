using Application.Recruitment.Features.CandidateExams.Models;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Exams;

namespace Application.Recruitment.Features.CandidateExams.Services;

public sealed class CandidateExamScoringService(IUnitOfWork unitOfWork)
{
    private const int QueryBatchSize = 500;

    public async Task<decimal?> CalculatePartScoreAsync(Guid attemptId, Guid partId, Guid examPartId,
        CancellationToken cancellationToken)
    {
        var scores = await CalculatePartCategoryScoresAsync(attemptId, partId, examPartId, cancellationToken);

        return scores is { Count: 1 } ? scores[0].Score : null;
    }

    public async Task<IReadOnlyList<CandidateExamCategoryScore>?> CalculatePartCategoryScoresAsync(Guid attemptId,
        Guid partId, Guid examPartId, CancellationToken cancellationToken)
    {
        var results = await CalculatePartCategoryScoresManyAsync([
            new CandidateExamPartScoringRequest(attemptId, partId, examPartId)
        ], cancellationToken);

        return results.TryGetValue(partId, out var scores) ? scores : null;
    }

    internal async Task<IReadOnlyDictionary<Guid, IReadOnlyList<CandidateExamCategoryScore>>>
        CalculatePartCategoryScoresManyAsync(IReadOnlyCollection<CandidateExamPartScoringRequest> requests,
            CancellationToken cancellationToken)
    {
        if (requests.Count == 0)
        {
            return new Dictionary<Guid, IReadOnlyList<CandidateExamCategoryScore>>();
        }

        var requestGroups = requests.GroupBy(x => x.TestAttemptPartId).ToList();

        if (requestGroups.Any(group => group.Select(x => new { x.TestAttemptId, x.ExamPartId }).Distinct().Count() > 1))
        {
            throw new InvalidOperationException("A test attempt part cannot have conflicting scoring requests.");
        }

        var distinctRequests = requestGroups.Select(group => group.First()).ToList();

        var examPartIds = distinctRequests.Select(x => x.ExamPartId).Distinct().ToArray();

        var categories = await unitOfWork.Context.Set<ExamCategory>()
            .AsNoTracking()
            .Where(category => examPartIds.Contains(category.ExamPartId))
            .Select(category => new
            {
                category.Id,
                category.ExamPartId,
                category.QuestionBankTypeId,
                category.WeightPercent,
                category.QuestionCount
            })
            .ToListAsync(cancellationToken);

        var answerAggregates = new List<PartCategoryAnswerAggregate>();

        foreach (var partIdBatch in distinctRequests.Select(x => x.TestAttemptPartId).Chunk(QueryBatchSize))
        {
            var partIds = partIdBatch.ToArray();

            var batch = await unitOfWork.Context.Set<TestAttemptQuestion>()
                .AsNoTracking()
                .Where(question => partIds.Contains(question.TestAttemptPartId))
                .GroupBy(question => new
                {
                    question.TestAttemptId, question.TestAttemptPartId, question.ExamCategoryId
                })
                .Select(group => new PartCategoryAnswerAggregate(group.Key.TestAttemptId, group.Key.TestAttemptPartId,
                    group.Key.ExamCategoryId, group.Count(),
                    group.Count(question =>
                        question.SelectedQuestionRevisionOption != null
                        && question.SelectedQuestionRevisionOption.QuestionRevisionId == question.QuestionRevisionId
                        && question.SelectedQuestionRevisionOption.IsCorrect)))
                .ToListAsync(cancellationToken);

            answerAggregates.AddRange(batch);
        }

        var categoriesByExamPart = categories.GroupBy(category => category.ExamPartId)
            .ToDictionary(group => group.Key, group => group.ToList());

        var answersByPart = answerAggregates.GroupBy(answer => answer.TestAttemptPartId)
            .ToDictionary(group => group.Key, group => group.ToList());

        var results = new Dictionary<Guid, IReadOnlyList<CandidateExamCategoryScore>>();

        foreach (var request in distinctRequests)
        {
            if (!categoriesByExamPart.TryGetValue(request.ExamPartId, out var partCategories)
                || partCategories.Count == 0 || partCategories.Any(category => category.QuestionCount <= 0)
                || partCategories.Select(category => category.QuestionBankTypeId).Distinct().Count()
                != partCategories.Count)
            {
                continue;
            }

            answersByPart.TryGetValue(request.TestAttemptPartId, out var partAnswers);

            partAnswers ??= [];

            if (partAnswers.Any(answer => answer.TestAttemptId != request.TestAttemptId))
            {
                continue;
            }

            var answersByCategory = partAnswers.ToDictionary(answer => answer.ExamCategoryId);

            var invalidQuestionDistribution = partCategories.Any(category =>
                !answersByCategory.TryGetValue(category.Id, out var answer)
                || answer.TotalCount != category.QuestionCount);

            if (invalidQuestionDistribution || partAnswers.Any(answer =>
                    partCategories.All(category => category.Id != answer.ExamCategoryId)))
            {
                continue;
            }

            var scores = partCategories.Select(category =>
                {
                    var answer = answersByCategory[category.Id];

                    return new CandidateExamCategoryScore(category.QuestionBankTypeId, category.WeightPercent,
                        decimal.Round(100m * answer.CorrectCount / category.QuestionCount, 2,
                            MidpointRounding.AwayFromZero));
                })
                .ToList();

            results[request.TestAttemptPartId] = scores;
        }

        return results;
    }
    
    private sealed record PartCategoryAnswerAggregate(
        Guid TestAttemptId,
        Guid TestAttemptPartId,
        Guid ExamCategoryId,
        int TotalCount,
        int CorrectCount);
}
