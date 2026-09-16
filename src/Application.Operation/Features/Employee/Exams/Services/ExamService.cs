using Application.Operation.Features.Employee.Exams.DTOs;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Employee.Exams.Services;

public sealed class ExamService(IUnitOfWork unitOfWork)
{
    public Task<bool> HasPendingApprovalExamForJobAsync(
        Guid jobId,
        Guid? excludeExamId,
        CancellationToken ct)
        => unitOfWork.Context.Set<Exam>().AsNoTracking()
            .AnyAsync(x => x.JobId == jobId &&
                x.StatusId == ExamStatusIds.PendingApproval &&
                (!excludeExamId.HasValue || x.Id != excludeExamId.Value), ct);

    public async Task<ExamConfigurationDto?> GetApprovedForJobAsync(Guid jobId, CancellationToken ct)
    {
        var exam = await unitOfWork.Context.Set<Exam>().AsNoTracking()
            .Where(x => x.JobId == jobId && x.StatusId == ExamStatusIds.Approved)
            .OrderByDescending(x => x.DecisionAt).ThenByDescending(x => x.CreatedDate).ThenBy(x => x.Id)
            .FirstOrDefaultAsync(ct);
        if (exam == null) return null;

        var dto = exam.Adapt<ExamConfigurationDto>();
        var parts = await unitOfWork.Context.Set<ExamPart>().AsNoTracking()
            .Where(x => x.ExamId == exam.Id).OrderBy(x => x.PartNo).ToListAsync(ct);
        var ids = parts.Select(x => x.Id).ToList();
        var categories = await unitOfWork.Context.Set<ExamCategory>().AsNoTracking()
            .Where(x => ids.Contains(x.ExamPartId)).OrderBy(x => x.QuestionBankTypeId).ToListAsync(ct);
        dto.Parts = parts.Select(part =>
        {
            var result = part.Adapt<ExamPartDto>();
            result.Categories = categories.Where(x => x.ExamPartId == part.Id).Adapt<List<ExamCategoryDto>>();
            return result;
        }).ToList();
        return dto;
    }
}
