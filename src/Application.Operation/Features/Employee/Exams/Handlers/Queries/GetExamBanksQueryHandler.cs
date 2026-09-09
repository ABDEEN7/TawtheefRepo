using Application.Operation.Features.Employee.Exams.DTOs;
using Application.Operation.Features.Employee.Exams.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.QuestionsBank;
using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Operation.Features.Employee.Exams.Handlers.Queries;

public sealed class GetExamBanksQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetExamBanksQuery, IResult<List<ExamBankDto>>>
{
    public async Task<IResult<List<ExamBankDto>>> Handle(GetExamBanksQuery request, CancellationToken ct)
    {
        var jobScope = await unitOfWork.Context.Set<Job>().AsNoTracking()
            .Where(x => x.Id == request.JobId)
            .Select(x => new { x.ManagementId, x.JobTitleId })
            .FirstOrDefaultAsync(ct);
        if (jobScope == null) return Result.Fail<List<ExamBankDto>>(ErrorsCodes.ItemNotFound);
        return Result.Ok(await ExamBankSelection.Load(
            unitOfWork, jobScope.ManagementId, jobScope.JobTitleId, ct));
    }
}

internal static class ExamBankSelection
{
    internal static async Task<List<ExamBankDto>> Load(IUnitOfWork unitOfWork, Guid managementId,
        Guid jobTitleId, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var eligibleVersions = unitOfWork.Context.Set<QuestionBankVersion>().AsNoTracking()
            .Where(v => !v.IsDeleted && v.ApprovedAt != null && v.ApprovedAt <= now &&
                        v.QuestionBank.IsActive && !v.QuestionBank.IsDeleted &&
                        (v.EffectiveFrom == null || v.EffectiveFrom <= now) &&
                        (v.EffectiveTo == null || v.EffectiveTo > now) &&
                        (v.QuestionBank.ManagementId == null || v.QuestionBank.ManagementId == managementId) &&
                        (v.QuestionBank.JobTitleId == null || v.QuestionBank.JobTitleId == jobTitleId) &&
                        (v.QuestionBank.QuestionBankTypeId == QuestionBankTypeIds.SPECIALIZED ||
                         v.QuestionBank.QuestionBankTypeId == QuestionBankTypeIds.EDUCATIONAL ||
                         v.QuestionBank.QuestionBankTypeId == QuestionBankTypeIds.SKILLS));

        var banks = await eligibleVersions
            .OrderBy(v => v.QuestionBank.QuestionBankType.NameAr).ThenByDescending(v => v.VersionNo)
            .Select(v => new
            {
                VersionId = v.Id,
                v.QuestionBank.QuestionBankTypeId,
                v.QuestionBank.QuestionBankType.NameAr,
                v.QuestionBank.QuestionBankType.NameEn,
                v.VersionNo
            })
            .ToListAsync(ct);

        var statistics = await unitOfWork.Context.Set<QuestionBankVersionQuestion>().AsNoTracking()
            .Where(q => eligibleVersions.Select(v => v.Id).Contains(q.QuestionBankVersionId) &&
                        !q.IsDeleted && !q.QuestionRevision.IsDeleted)
            .GroupBy(q => q.QuestionBankVersionId)
            .Select(g => new
            {
                QuestionBankVersionId = g.Key,
                Total = g.Count(),
                Easy = g.Count(q => q.QuestionRevision.DifficultyLevelId == DifficultyLevelIds.EASY),
                Medium = g.Count(q => q.QuestionRevision.DifficultyLevelId == DifficultyLevelIds.MEDIUM),
                Hard = g.Count(q => q.QuestionRevision.DifficultyLevelId == DifficultyLevelIds.HARD)
            })
            .ToListAsync(ct);

        var statisticsByVersionId = statistics.ToDictionary(x => x.QuestionBankVersionId);
        return banks.Select(bank =>
        {
            statisticsByVersionId.TryGetValue(bank.VersionId, out var bankStatistics);
            return new ExamBankDto(
                bank.VersionId,
                bank.QuestionBankTypeId == QuestionBankTypeIds.SPECIALIZED ? ExamCategoryTypeIds.Specialized :
                bank.QuestionBankTypeId == QuestionBankTypeIds.EDUCATIONAL ? ExamCategoryTypeIds.Educational :
                ExamCategoryTypeIds.Skills,
                bank.NameAr,
                bank.NameEn,
                bank.VersionNo,
                bankStatistics?.Total ?? 0,
                bankStatistics?.Easy ?? 0,
                bankStatistics?.Medium ?? 0,
                bankStatistics?.Hard ?? 0);
        }).ToList();
    }
}
