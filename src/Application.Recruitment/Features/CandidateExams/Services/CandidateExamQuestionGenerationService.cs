using System.Security.Cryptography;
using Application.Recruitment.Features.CandidateExams.DTOs;
using Application.Recruitment.Features.CandidateExams.Models;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Concurrency;
using Tawtheef.Application.Common.Interfaces.Concurrency;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.QuestionsBank;

namespace Application.Recruitment.Features.CandidateExams.Services;

public sealed class CandidateExamQuestionGenerationService(
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    IApplicationLockService applicationLockService)
{
    public async Task<IResult<CandidateExamQuestionGenerationResult>> ValidateCompleteSetAsync(Guid testAttemptId,
        CancellationToken cancellationToken)
    {
        var attempt = await unitOfWork.Context.Set<TestAttempt>()
            .AsNoTracking()
            .Where(x => x.Id == testAttemptId)
            .Select(x => new CandidateExamQuestionGenerationAttemptData(x.Id, x.StatusId,
                x.TestSessionCandidate!.TestSession!.ExamId, x.TestSessionCandidate.TestSession.Exam!.StatusId,
                x.TestSessionCandidate.TestSession.Exam.TotalQuestions))
            .SingleOrDefaultAsync(cancellationToken);
        if (attempt is null) return Fail(ErrorsCodes.CandidateExamAttemptNotFound);

        var examParts = await unitOfWork.Context.Set<ExamPart>()
            .AsNoTracking()
            .Where(x => x.ExamId == attempt.ExamId)
            .OrderBy(x => x.PartNo)
            .Select(x => new CandidateExamQuestionGenerationExamPartData(x.Id, x.PartNo))
            .ToListAsync(cancellationToken);
        var attemptParts = await unitOfWork.Context.Set<TestAttemptPart>()
            .AsNoTracking()
            .Where(x => x.TestAttemptId == testAttemptId)
            .Select(x => new CandidateExamQuestionGenerationAttemptPartData(x.Id, x.ExamPartId, x.StatusId, x.StartedAt,
                x.CompletedAt, x.DurationMinutesSnapshot, x.EffectiveDeadlineAt, x.LastQuestionNo))
            .ToListAsync(cancellationToken);
        if (!PartAssociationsAreComplete(examParts, attemptParts))
            return Fail(ErrorsCodes.CandidateExamAttemptStructureInvalid);

        var partIds = examParts.Select(x => x.Id).ToList();
        var categories = await unitOfWork.Context.Set<ExamCategory>()
            .AsNoTracking()
            .Where(x => partIds.Contains(x.ExamPartId))
            .OrderBy(x => x.ExamPart!.PartNo)
            .ThenBy(x => x.Id)
            .Select(x => new CandidateExamQuestionGenerationCategoryData(x.Id, x.ExamPartId, x.QuestionBankTypeId,
                x.QuestionBankVersionId, x.QuestionCount, x.WeightPercent, x.EasyQuestionCount, x.MediumQuestionCount,
                x.HardQuestionCount, x.QuestionBankVersion!.ApprovedAt,
                x.QuestionBankVersion.QuestionBank.QuestionBankTypeId))
            .ToListAsync(cancellationToken);
        if (!ConfigurationIsValid(attempt.TotalQuestions, examParts, categories))
            return Fail(ErrorsCodes.CandidateExamConfigurationInvalid);

        var versionIds = categories.Select(x => x.QuestionBankVersionId).Distinct().ToList();
        var memberships = await unitOfWork.Context.Set<QuestionBankVersionQuestion>()
            .AsNoTracking()
            .Where(x => versionIds.Contains(x.QuestionBankVersionId))
            .Select(x => new CandidateExamQuestionGenerationMembershipData(x.QuestionBankVersionId, x.QuestionId,
                x.QuestionRevisionId, x.QuestionRevision.QuestionId, x.QuestionRevision.DifficultyLevelId))
            .ToListAsync(cancellationToken);
        if (memberships.Any(x => x.QuestionId != x.RevisionQuestionId))
            return Fail(ErrorsCodes.CandidateExamQuestionRevisionMismatch);

        var existing = await unitOfWork.Context.Set<TestAttemptQuestion>()
            .AsNoTracking()
            .Where(x => x.TestAttemptId == testAttemptId)
            .Select(x => new CandidateExamQuestionGenerationExistingQuestionData(x.TestAttemptPartId, x.ExamCategoryId,
                x.QuestionId, x.QuestionRevisionId, x.QuestionRevision!.DifficultyLevelId, x.OrderNo,
                x.SelectedQuestionRevisionOptionId, x.SavedAt))
            .ToListAsync(cancellationToken);
        return ExistingSetResult(attempt, examParts, attemptParts, categories, memberships, existing);
    }

    public Task<IResult<CandidateExamQuestionGenerationResult>> GenerateAsync(Guid testAttemptId,
        CancellationToken cancellationToken) =>
        unitOfWork.ExecuteInTransactionAsync<IResult<CandidateExamQuestionGenerationResult>>(async token =>
        {
            await applicationLockService.AcquireExclusiveAsync(
                ApplicationLockResources.CandidateExamAttempt(testAttemptId),
                token);

            var attempt = await unitOfWork.Context.Set<TestAttempt>()
                .AsNoTracking()
                .Where(x => x.Id == testAttemptId)
                .Select(x => new CandidateExamQuestionGenerationAttemptData(x.Id, x.StatusId,
                    x.TestSessionCandidate!.TestSession!.ExamId, x.TestSessionCandidate.TestSession.Exam!.StatusId,
                    x.TestSessionCandidate.TestSession.Exam.TotalQuestions))
                .SingleOrDefaultAsync(token);

            if (attempt is null)
                return Fail(ErrorsCodes.CandidateExamAttemptNotFound);
            if (attempt.StatusId != TestAttemptStatusIds.NotStarted)
                return Fail(AttemptStatusError(attempt.StatusId));
            if (attempt.ExamStatusId != ExamStatusIds.Approved)
                return Fail(ErrorsCodes.CandidateExamConfigurationInvalid);

            var examParts = await unitOfWork.Context.Set<ExamPart>()
                .AsNoTracking()
                .Where(x => x.ExamId == attempt.ExamId)
                .OrderBy(x => x.PartNo)
                .Select(x => new CandidateExamQuestionGenerationExamPartData(x.Id, x.PartNo))
                .ToListAsync(token);
            var attemptParts = await unitOfWork.Context.Set<TestAttemptPart>()
                .AsNoTracking()
                .Where(x => x.TestAttemptId == testAttemptId)
                .Select(x => new CandidateExamQuestionGenerationAttemptPartData(x.Id, x.ExamPartId, x.StatusId,
                    x.StartedAt, x.CompletedAt, x.DurationMinutesSnapshot, x.EffectiveDeadlineAt, x.LastQuestionNo))
                .ToListAsync(token);

            if (!PreparedStructureIsValid(examParts, attemptParts))
                return Fail(ErrorsCodes.CandidateExamAttemptStructureInvalid);

            var examPartIds = examParts.Select(x => x.Id).ToList();
            var categories = await unitOfWork.Context.Set<ExamCategory>()
                .AsNoTracking()
                .Where(x => examPartIds.Contains(x.ExamPartId))
                .OrderBy(x => x.ExamPart!.PartNo)
                .ThenBy(x => x.Id)
                .Select(x => new CandidateExamQuestionGenerationCategoryData(x.Id, x.ExamPartId, x.QuestionBankTypeId,
                    x.QuestionBankVersionId, x.QuestionCount, x.WeightPercent, x.EasyQuestionCount,
                    x.MediumQuestionCount, x.HardQuestionCount, x.QuestionBankVersion!.ApprovedAt,
                    x.QuestionBankVersion.QuestionBank.QuestionBankTypeId))
                .ToListAsync(token);

            if (!ConfigurationIsValid(attempt.TotalQuestions, examParts, categories))
                return Fail(ErrorsCodes.CandidateExamConfigurationInvalid);
            var now = timeProvider.GetUtcNow().UtcDateTime;
            if (categories.Any(x => x.VersionApprovedAt is null || x.VersionApprovedAt > now))
                return Fail(ErrorsCodes.CandidateExamQuestionBankVersionInvalid);
            if (categories.Any(x => x.QuestionBankTypeId != x.VersionQuestionBankTypeId))
                return Fail(ErrorsCodes.CandidateExamQuestionBankTypeMismatch);

            var versionIds = categories.Select(x => x.QuestionBankVersionId).Distinct().ToList();
            var memberships = await unitOfWork.Context.Set<QuestionBankVersionQuestion>()
                .AsNoTracking()
                .Where(x => versionIds.Contains(x.QuestionBankVersionId))
                .Select(x => new CandidateExamQuestionGenerationMembershipData(x.QuestionBankVersionId, x.QuestionId,
                    x.QuestionRevisionId, x.QuestionRevision.QuestionId, x.QuestionRevision.DifficultyLevelId))
                .ToListAsync(token);

            if (memberships.Any(x => x.QuestionId != x.RevisionQuestionId))
                return Fail(ErrorsCodes.CandidateExamQuestionRevisionMismatch);

            var existing = await unitOfWork.Context.Set<TestAttemptQuestion>()
                .AsNoTracking()
                .Where(x => x.TestAttemptId == testAttemptId)
                .Select(x => new CandidateExamQuestionGenerationExistingQuestionData(x.TestAttemptPartId,
                    x.ExamCategoryId, x.QuestionId, x.QuestionRevisionId, x.QuestionRevision!.DifficultyLevelId,
                    x.OrderNo, x.SelectedQuestionRevisionOptionId, x.SavedAt))
                .ToListAsync(token);

            if (existing.Count > 0)
                return ExistingSetResult(attempt, examParts, attemptParts, categories, memberships, existing);

            var slots = BuildQuotaSlots(categories, memberships);
            if (slots is null || !TryAllocateDistinctQuestions(slots, out var selected))
                return Fail(ErrorsCodes.CandidateExamInsufficientQuestions);

            var attemptPartByExamPart = attemptParts.ToDictionary(x => x.ExamPartId);
            var categoryById = categories.ToDictionary(x => x.Id);
            var questions = new List<TestAttemptQuestion>(selected.Count);
            foreach (var partGroup in selected.GroupBy(x => categoryById[x.CategoryId].ExamPartId))
            {
                var partQuestions = partGroup.ToList();
                Shuffle(partQuestions);
                var attemptPart = attemptPartByExamPart[partGroup.Key];
                for (var index = 0; index < partQuestions.Count; index++)
                {
                    var item = partQuestions[index];
                    questions.Add(new TestAttemptQuestion
                    {
                        TestAttemptId = testAttemptId,
                        TestAttemptPartId = attemptPart.Id,
                        ExamCategoryId = item.CategoryId,
                        QuestionId = item.Membership.QuestionId,
                        QuestionRevisionId = item.Membership.QuestionRevisionId,
                        OrderNo = index + 1,
                        SelectedQuestionRevisionOptionId = null,
                        SavedAt = null
                    });
                }
            }

            await unitOfWork.Context.Set<TestAttemptQuestion>().AddRangeAsync(questions, token);
            await unitOfWork.SaveChangesAsync(token);
            return Result.Ok(new CandidateExamQuestionGenerationResult(
                testAttemptId, true, questions.Count, attemptParts.Count));
        }, cancellationToken);

    private static bool PreparedStructureIsValid(
        IReadOnlyCollection<CandidateExamQuestionGenerationExamPartData> examParts,
        IReadOnlyCollection<CandidateExamQuestionGenerationAttemptPartData> attemptParts) =>
        PartAssociationsAreComplete(examParts, attemptParts) && attemptParts.All(x =>
            x.StatusId == TestAttemptPartStatusIds.NotStarted && x.StartedAt is null && x.CompletedAt is null
            && x.DurationMinutesSnapshot is null && x.EffectiveDeadlineAt is null && x.LastQuestionNo is null);

    private static bool PartAssociationsAreComplete(
        IReadOnlyCollection<CandidateExamQuestionGenerationExamPartData> examParts,
        IReadOnlyCollection<CandidateExamQuestionGenerationAttemptPartData> attemptParts) =>
        examParts.Count > 0 && attemptParts.Count == examParts.Count
                            && attemptParts.Select(x => x.ExamPartId).Distinct().Count() == attemptParts.Count
                            && !examParts.Select(x => x.Id).Except(attemptParts.Select(x => x.ExamPartId)).Any();

    private static bool ConfigurationIsValid(int totalQuestions,
        IReadOnlyCollection<CandidateExamQuestionGenerationExamPartData> parts,
        IReadOnlyCollection<CandidateExamQuestionGenerationCategoryData> categories) =>
        categories.Count > 0
        && categories.All(x => x.QuestionCount > 0 && x.WeightPercent is >= 0 and <= 100 && x.EasyQuestionCount >= 0
                               && x.MediumQuestionCount >= 0 && x.HardQuestionCount >= 0
                               && (long)x.EasyQuestionCount + x.MediumQuestionCount + x.HardQuestionCount
                               == x.QuestionCount) && categories.Sum(x => x.WeightPercent) == 100m
        && categories.Sum(x => (long)x.QuestionCount) == totalQuestions
        && parts.All(part => categories.Any(category => category.ExamPartId == part.Id)) && categories
            .GroupBy(x => x.ExamPartId)
            .All(group => group.Select(x => x.QuestionBankTypeId).Distinct().Count() == group.Count());

    private static List<CandidateExamQuestionGenerationQuotaSlot>? BuildQuotaSlots(
        IReadOnlyCollection<CandidateExamQuestionGenerationCategoryData> categories,
        IReadOnlyCollection<CandidateExamQuestionGenerationMembershipData> memberships)
    {
        var slots = new List<CandidateExamQuestionGenerationQuotaSlot>();
        foreach (var category in categories)
        {
            foreach (var quota in new[]
                     {
                         (DifficultyLevelIds.EASY, category.EasyQuestionCount),
                         (DifficultyLevelIds.MEDIUM, category.MediumQuestionCount),
                         (DifficultyLevelIds.HARD, category.HardQuestionCount)
                     })
            {
                var candidates = memberships.Where(x =>
                        x.QuestionBankVersionId == category.QuestionBankVersionId && x.DifficultyLevelId == quota.Item1)
                    .ToList();
                if (candidates.Select(x => x.QuestionId).Distinct().Count() < quota.Item2)
                    return null;
                Shuffle(candidates);
                for (var i = 0; i < quota.Item2; i++)
                    slots.Add(new CandidateExamQuestionGenerationQuotaSlot(category.Id, candidates));
            }
        }

        Shuffle(slots);
        return slots;
    }

    private static bool TryAllocateDistinctQuestions(IReadOnlyList<CandidateExamQuestionGenerationQuotaSlot> slots,
        out List<CandidateExamQuestionGenerationSelectedQuestion> selected)
    {
        var matchedQuestionSlots = new Dictionary<Guid, int>();
        var slotAssignments = new CandidateExamQuestionGenerationMembershipData?[slots.Count];
        for (var slotIndex = 0; slotIndex < slots.Count; slotIndex++)
            if (!TryMatch(slotIndex, slots, matchedQuestionSlots, slotAssignments, []))
            {
                selected = [];
                return false;
            }

        selected = slots.Select((slot, index) =>
                new CandidateExamQuestionGenerationSelectedQuestion(slot.CategoryId, slotAssignments[index]!))
            .ToList();
        return true;
    }

    private static bool TryMatch(int slotIndex, IReadOnlyList<CandidateExamQuestionGenerationQuotaSlot> slots,
        IDictionary<Guid, int> matchedQuestionSlots, IList<CandidateExamQuestionGenerationMembershipData?> assignments,
        HashSet<Guid> visitedQuestions)
    {
        foreach (var candidate in slots[slotIndex].Candidates)
        {
            if (!visitedQuestions.Add(candidate.QuestionId)) continue;
            if (!matchedQuestionSlots.TryGetValue(candidate.QuestionId, out var occupiedSlot) || TryMatch(occupiedSlot,
                    slots, matchedQuestionSlots, assignments, visitedQuestions))
            {
                matchedQuestionSlots[candidate.QuestionId] = slotIndex;
                assignments[slotIndex] = candidate;
                return true;
            }
        }

        return false;
    }

    private static IResult<CandidateExamQuestionGenerationResult> ExistingSetResult(
        CandidateExamQuestionGenerationAttemptData attempt,
        IReadOnlyCollection<CandidateExamQuestionGenerationExamPartData> examParts,
        IReadOnlyCollection<CandidateExamQuestionGenerationAttemptPartData> attemptParts,
        IReadOnlyCollection<CandidateExamQuestionGenerationCategoryData> categories,
        IReadOnlyCollection<CandidateExamQuestionGenerationMembershipData> memberships,
        IReadOnlyCollection<CandidateExamQuestionGenerationExistingQuestionData> existing)
    {
        var partById = attemptParts.ToDictionary(x => x.Id);
        var categoryById = categories.ToDictionary(x => x.Id);
        var membershipByKey = memberships.ToDictionary(x => (x.QuestionBankVersionId, x.QuestionId));
        var valid = existing.Count == attempt.TotalQuestions
                    && existing.Select(x => x.QuestionId).Distinct().Count() == existing.Count && existing.All(x =>
                        x.SelectedOptionId is null && x.SavedAt is null
                                                   && partById.TryGetValue(x.TestAttemptPartId, out var part)
                                                   && categoryById.TryGetValue(x.ExamCategoryId, out var category)
                                                   && category.ExamPartId == part.ExamPartId
                                                   && membershipByKey.TryGetValue(
                                                       (category.QuestionBankVersionId, x.QuestionId),
                                                       out var membership)
                                                   && membership.QuestionRevisionId == x.QuestionRevisionId
                                                   && membership.DifficultyLevelId == x.DifficultyLevelId)
                    && categories.All(category => CategoryCountIsValid(category, existing))
                    && attemptParts.All(part => PartOrderIsValid(part.Id, categories, existing));

        return valid
            ? Result.Ok(new CandidateExamQuestionGenerationResult(attempt.Id, false, existing.Count, examParts.Count))
            : Fail(ErrorsCodes.CandidateExamExistingQuestionSetInvalid);
    }

    private static bool CategoryCountIsValid(CandidateExamQuestionGenerationCategoryData category,
        IEnumerable<CandidateExamQuestionGenerationExistingQuestionData> existing)
    {
        var rows = existing.Where(x => x.ExamCategoryId == category.Id).ToList();
        return rows.Count == category.QuestionCount
               && rows.Count(x => x.DifficultyLevelId == DifficultyLevelIds.EASY) == category.EasyQuestionCount
               && rows.Count(x => x.DifficultyLevelId == DifficultyLevelIds.MEDIUM) == category.MediumQuestionCount
               && rows.Count(x => x.DifficultyLevelId == DifficultyLevelIds.HARD) == category.HardQuestionCount;
    }

    private static bool PartOrderIsValid(Guid attemptPartId,
        IReadOnlyCollection<CandidateExamQuestionGenerationCategoryData> categories,
        IEnumerable<CandidateExamQuestionGenerationExistingQuestionData> existing)
    {
        var categoryIds = categories.Select(x => x.Id).ToHashSet();
        var order = existing.Where(x => x.TestAttemptPartId == attemptPartId && categoryIds.Contains(x.ExamCategoryId))
            .Select(x => x.OrderNo)
            .Order()
            .ToList();
        return order.SequenceEqual(Enumerable.Range(1, order.Count));
    }

    private static void Shuffle<T>(IList<T> items)
    {
        for (var i = items.Count - 1; i > 0; i--)
        {
            var j = RandomNumberGenerator.GetInt32(i + 1);
            (items[i], items[j]) = (items[j], items[i]);
        }
    }

    private static string AttemptStatusError(Guid statusId) =>
        statusId == TestAttemptStatusIds.InProgress ? ErrorsCodes.CandidateExamAttemptAlreadyStarted :
        statusId == TestAttemptStatusIds.Interrupted ? ErrorsCodes.CandidateExamAttemptInterrupted :
        statusId == TestAttemptStatusIds.Completed ? ErrorsCodes.CandidateExamAttemptCompleted :
        statusId == TestAttemptStatusIds.TimedOut ? ErrorsCodes.CandidateExamAttemptTimedOut :
        statusId == TestAttemptStatusIds.Cancelled ? ErrorsCodes.CandidateExamAttemptCancelled :
        ErrorsCodes.CandidateExamAttemptStatusInvalid;

    private static IResult<CandidateExamQuestionGenerationResult> Fail(string code) =>
        Result.Fail<CandidateExamQuestionGenerationResult>(code);
}
