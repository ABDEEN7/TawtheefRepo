using Application.Recruitment.Features.CandidateExams.Models;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Recruitment.Features.CandidateExams.Services;

public sealed class CandidateExamResultCalculationService(
    IUnitOfWork unitOfWork,
    CandidateExamScoringService scoringService)
{
    private const int QueryBatchSize = 500;

    public async Task<CandidateExamCalculatedScores?> CalculateAsync(Guid testAttemptId,
        CancellationToken cancellationToken)
    {
        var results = await CalculateManyAsync([testAttemptId], cancellationToken);

        return results.TryGetValue(testAttemptId, out var calculated) ? calculated : null;
    }

    internal async Task<IReadOnlyDictionary<Guid, CandidateExamCalculatedScores>> CalculateManyAsync(
        IReadOnlyCollection<Guid> testAttemptIds, CancellationToken cancellationToken)
    {
        var attemptIds = testAttemptIds.Where(id => id != Guid.Empty).Distinct().ToArray();

        if (attemptIds.Length == 0)
        {
            return new Dictionary<Guid, CandidateExamCalculatedScores>();
        }

        var attempts = new List<ResultAttemptState>();

        var parts = new List<ResultPartState>();

        foreach (var attemptIdBatch in attemptIds.Chunk(QueryBatchSize))
        {
            var batchIds = attemptIdBatch.ToArray();

            attempts.AddRange(await unitOfWork.Context.Set<TestAttempt>()
                .AsNoTracking()
                .Where(attempt => batchIds.Contains(attempt.Id))
                .Select(attempt => new ResultAttemptState(attempt.Id, attempt.StatusId, attempt.CompletedAt))
                .ToListAsync(cancellationToken));

            parts.AddRange(await unitOfWork.Context.Set<TestAttemptPart>()
                .AsNoTracking()
                .Where(part => batchIds.Contains(part.TestAttemptId))
                .Select(part => new ResultPartState(part.Id, part.TestAttemptId, part.ExamPartId, part.StatusId,
                    part.CompletedAt, part.ExamPart!.PartNo, part.ExamPart.QualificationScore))
                .ToListAsync(cancellationToken));
        }

        var attemptsById = attempts.ToDictionary(attempt => attempt.Id);

        var partsByAttempt = parts.GroupBy(part => part.TestAttemptId)
            .ToDictionary(group => group.Key, group => group.OrderBy(part => part.PartNo).ToArray());

        var validStructures = new Dictionary<Guid, ResultPartState[]>();

        var scoringRequests = new List<CandidateExamPartScoringRequest>();

        foreach (var attemptId in attemptIds)
        {
            if (!attemptsById.TryGetValue(attemptId, out var attempt) || attempt.CompletedAt is null
                                                                      || (attempt.StatusId
                                                                          != TestAttemptStatusIds.Completed
                                                                          && attempt.StatusId
                                                                          != TestAttemptStatusIds.TimedOut))
            {
                continue;
            }

            if (!partsByAttempt.TryGetValue(attemptId, out var attemptParts) || attemptParts.Length != 2
                                                                             || attemptParts[0].PartNo != 1
                                                                             || attemptParts[1].PartNo != 2
                                                                             || attemptParts[0]
                                                                                 .QualificationScore is null
                                                                             || attemptParts[0].QualificationScore is
                                                                                 < 0 or > 100
                                                                             || !IsFinished(attemptParts[0].StatusId)
                                                                             || attemptParts[0].CompletedAt is null)
            {
                continue;
            }

            validStructures[attemptId] = attemptParts;

            scoringRequests.Add(
                new CandidateExamPartScoringRequest(attemptId, attemptParts[0].Id, attemptParts[0].ExamPartId));

            if (IsFinished(attemptParts[1].StatusId) && attemptParts[1].CompletedAt is not null)
            {
                scoringRequests.Add(new CandidateExamPartScoringRequest(attemptId, attemptParts[1].Id,
                    attemptParts[1].ExamPartId));
            }
        }

        var partScores = await scoringService.CalculatePartCategoryScoresManyAsync(scoringRequests, cancellationToken);

        var results = new Dictionary<Guid, CandidateExamCalculatedScores>();

        foreach (var pair in validStructures)
        {
            var attemptId = pair.Key;
            var attemptParts = pair.Value;

            var specializedPart = attemptParts[0];

            var remainingPart = attemptParts[1];

            if (!partScores.TryGetValue(specializedPart.Id, out var specializedScores)
                || specializedScores is not { Count: 1 }
                || specializedScores[0].QuestionBankTypeId != QuestionBankTypeIds.Specialized)
            {
                continue;
            }

            var specialized = specializedScores[0];

            var specializedThreshold = specializedPart.QualificationScore!.Value;

            var isSpecializedQualified = specialized.Score >= specializedThreshold;

            if (!isSpecializedQualified)
            {
                if (remainingPart.StatusId != TestAttemptPartStatusIds.NotStarted
                    || remainingPart.CompletedAt is not null)
                {
                    continue;
                }

                results[attemptId] =
                    new CandidateExamCalculatedScores(specialized.Score, specializedThreshold, false, null, null, null);

                continue;
            }

            if (!IsFinished(remainingPart.StatusId) || remainingPart.CompletedAt is null
                                                    || !partScores.TryGetValue(remainingPart.Id,
                                                        out var remainingScores) || remainingScores is not { Count: 2 })
            {
                continue;
            }

            var educational =
                remainingScores.SingleOrDefault(score => score.QuestionBankTypeId == QuestionBankTypeIds.Educational);

            var skills =
                remainingScores.SingleOrDefault(score => score.QuestionBankTypeId == QuestionBankTypeIds.Skills);

            if (educational is null || skills is null)
            {
                continue;
            }

            var totalWeight = specialized.WeightPercent + educational.WeightPercent + skills.WeightPercent;

            if (totalWeight != 100m)
            {
                continue;
            }

            var finalScore =
                decimal.Round(
                    (specialized.Score * specialized.WeightPercent + educational.Score * educational.WeightPercent
                                                                   + skills.Score * skills.WeightPercent) / 100m, 2,
                    MidpointRounding.AwayFromZero);

            results[attemptId] = new CandidateExamCalculatedScores(specialized.Score, specializedThreshold, true,
                educational.Score, skills.Score, finalScore);
        }

        return results;
    }

    private static bool IsFinished(Guid statusId) =>
        statusId == TestAttemptPartStatusIds.Completed || statusId == TestAttemptPartStatusIds.TimedOut;

    private sealed record ResultAttemptState(Guid Id, Guid StatusId, DateTime? CompletedAt);

    private sealed record ResultPartState(
        Guid Id,
        Guid TestAttemptId,
        Guid ExamPartId,
        Guid StatusId,
        DateTime? CompletedAt,
        int PartNo,
        decimal? QualificationScore);
}
