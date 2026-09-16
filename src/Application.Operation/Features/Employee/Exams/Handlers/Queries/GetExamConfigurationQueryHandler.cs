using Application.Operation.Features.Employee.Exams.DTOs;
using Application.Operation.Features.Employee.Exams.Queries;
using FluentResults;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Employee.Exams.Handlers.Queries;

public sealed class GetExamConfigurationQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetExamConfigurationQuery, IResult<ExamConfigurationDto>>
{
    public async Task<IResult<ExamConfigurationDto>> Handle(GetExamConfigurationQuery request,
        CancellationToken ct)
    {
        var exam = await unitOfWork.Context.Set<Exam>().AsNoTracking().Include(x => x.Status).Include(x => x.DecisionBy)
            .FirstOrDefaultAsync(x => x.Id == request.ExamId &&
                (request.IsViewMode || x.StatusId == ExamStatusIds.Draft || x.StatusId == ExamStatusIds.Returned), ct);
        if (exam == null) return Result.Fail<ExamConfigurationDto>(ErrorsCodes.InvalidRequest);

        var dto = exam.Adapt<ExamConfigurationDto>();
        dto.StatusBackendName = exam.Status?.BackendName;
        dto.DecisionAt = exam.DecisionAt;
        dto.DecisionByName = exam.DecisionBy?.FullNameEn ?? exam.DecisionBy?.FullNameAr;
        var parts = await unitOfWork.Context.Set<ExamPart>().AsNoTracking()
            .Where(x => x.ExamId == exam.Id).OrderBy(x => x.PartNo).ToListAsync(ct);
        var partIds = parts.Select(x => x.Id).ToList();
        var categories = await unitOfWork.Context.Set<ExamCategory>().AsNoTracking()
            .Where(x => partIds.Contains(x.ExamPartId)).OrderBy(x => x.QuestionBankTypeId).ToListAsync(ct);
        dto.Parts = parts.Select(part =>
        {
            var result = part.Adapt<ExamPartDto>();
            result.Categories = categories.Where(x => x.ExamPartId == part.Id).Adapt<List<ExamCategoryDto>>();
            return result;
        }).ToList();
        return Result.Ok(dto);
    }
}
