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
    public async Task<CandidateExamCalculatedScores?> CalculateAsync(
        Guid testAttemptId,
        CancellationToken cancellationToken)
    {
        var attempt = await unitOfWork.Context
            .Set<TestAttempt>()
            .AsNoTracking()
            .Where(x => x.Id == testAttemptId)
            .Select(x => new { x.StatusId, x.CompletedAt })
            .SingleOrDefaultAsync(cancellationToken);

        if (attempt?.CompletedAt is null ||
            (attempt.StatusId != TestAttemptStatusIds.Completed &&
             attempt.StatusId != TestAttemptStatusIds.TimedOut))
        {
            return null;
        }

        var parts = await unitOfWork.Context
            .Set<TestAttemptPart>()
            .AsNoTracking()
            .Where(x => x.TestAttemptId == testAttemptId)
            .OrderBy(x => x.ExamPart!.PartNo)
            .Select(x => new
            {
                x.Id,
                x.StatusId,
                x.CompletedAt,
                x.ExamPart!.PartNo,
                x.ExamPart.QualificationScore
            })
            .ToListAsync(cancellationToken);

        if (parts.Count != 2 ||
            parts[0].PartNo != 1 ||
            parts[1].PartNo != 2 ||
            parts[0].QualificationScore is null ||
            parts[0].QualificationScore is < 0 or > 100 ||
            !IsFinished(parts[0].StatusId) ||
            parts[0].CompletedAt is null)
        {
            return null;
        }

        var specializedScores =
            await scoringService.CalculatePartCategoryScoresAsync(
                testAttemptId,
                parts[0].Id,
                cancellationToken);

        if (specializedScores is not { Count: 1 } || specializedScores[0].QuestionBankTypeId != QuestionBankTypeIds.Specialized)
        {
            return null;
        }

        var specialized = specializedScores[0];
        var specializedQualificationThreshold = parts[0].QualificationScore!.Value;
        var isSpecializedQualified = specialized.Score >= specializedQualificationThreshold;

        if (!isSpecializedQualified)
        {
            if (parts[1].StatusId != TestAttemptPartStatusIds.NotStarted || parts[1].CompletedAt is not null)
            {
                return null;
            }

            return new CandidateExamCalculatedScores(
                SpecializedScore: specialized.Score,
                SpecializedQualificationThreshold:
                specializedQualificationThreshold,
                IsSpecializedQualified: false,
                EducationalScore: null,
                SkillsScore: null,
                FinalScore: null);
        }

        if (!IsFinished(parts[1].StatusId) || parts[1].CompletedAt is null)
        {
            return null;
        }

        var remainingScores =
            await scoringService.CalculatePartCategoryScoresAsync(
                testAttemptId,
                parts[1].Id,
                cancellationToken);

        if (remainingScores is not { Count: 2 })
        {
            return null;
        }

        var educational = remainingScores.SingleOrDefault(x => x.QuestionBankTypeId ==
                                                               QuestionBankTypeIds.Educational);

        var skills = remainingScores.SingleOrDefault(x => x.QuestionBankTypeId ==
                                                          QuestionBankTypeIds.Skills);

        if (educational is null || skills is null)
        {
            return null;
        }

        var totalWeight = specialized.WeightPercent + educational.WeightPercent + skills.WeightPercent;

        if (totalWeight != 100m)
        {
            return null;
        }

        var finalScore = decimal.Round(
            (
                specialized.Score *
                specialized.WeightPercent +
                educational.Score *
                educational.WeightPercent +
                skills.Score *
                skills.WeightPercent
            ) / 100m,
            2,
            MidpointRounding.AwayFromZero);

        return new CandidateExamCalculatedScores(
            SpecializedScore: specialized.Score,
            SpecializedQualificationThreshold:
            specializedQualificationThreshold,
            IsSpecializedQualified: true,
            EducationalScore: educational.Score,
            SkillsScore: skills.Score,
            FinalScore: finalScore);
    }

    private static bool IsFinished(Guid statusId) =>
        statusId == TestAttemptPartStatusIds.Completed ||
        statusId == TestAttemptPartStatusIds.TimedOut;
}
