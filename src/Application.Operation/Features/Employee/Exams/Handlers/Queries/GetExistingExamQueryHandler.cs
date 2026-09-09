using Application.Operation.Features.Employee.Exams.DTOs;
using Application.Operation.Features.Employee.Exams.Queries;
using FluentResults;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Employee.Exams.Handlers.Queries;

public sealed class GetExistingExamQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetExistingExamQuery, IResult<ExamConfigurationDto?>>
{
    public async Task<IResult<ExamConfigurationDto?>> Handle(GetExistingExamQuery request, CancellationToken ct)
    {
        var exam = await unitOfWork.Context.Set<Exam>().AsNoTracking()
            .Where(x => x.JobId == request.JobId && x.StatusId == ExamStatusIds.Approved)
            .OrderByDescending(x => x.ApprovedAt).ThenByDescending(x => x.CreatedDate).ThenBy(x => x.Id)
            .FirstOrDefaultAsync(ct);
        if (exam == null) return Result.Ok<ExamConfigurationDto?>(null);
        var dto = exam.Adapt<ExamConfigurationDto>();
        var parts = await unitOfWork.Context.Set<ExamPart>().AsNoTracking()
            .Where(x => x.ExamId == exam.Id).OrderBy(x => x.PartNo).ToListAsync(ct);
        var ids = parts.Select(x => x.Id).ToList();
        var categories = await unitOfWork.Context.Set<ExamCategory>().AsNoTracking()
            .Where(x => ids.Contains(x.ExamPartId)).OrderBy(x => x.CategoryId).ToListAsync(ct);
        dto.Parts = parts.Select(part =>
        {
            var result = part.Adapt<ExamPartDto>();
            result.Categories = categories.Where(x => x.ExamPartId == part.Id).Adapt<List<ExamCategoryDto>>();
            return result;
        }).ToList();
        return Result.Ok<ExamConfigurationDto?>(dto);
    }
}
