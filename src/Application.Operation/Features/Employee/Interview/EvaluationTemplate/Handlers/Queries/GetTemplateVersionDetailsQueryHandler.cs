using Application.Operation.Features.Employee.Interview.EvaluationTemplate.DTOs;
using Application.Operation.Features.Employee.Interview.EvaluationTemplate.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.EvaluationTemplate.Handlers.Queries;

public sealed class GetTemplateVersionDetailsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetTemplateVersionDetailsQuery, IResult<TemplateVersionDetailsDto>>
{
    public async Task<IResult<TemplateVersionDetailsDto>> Handle(GetTemplateVersionDetailsQuery request, CancellationToken cancellationToken)
    {
        var version = await unitOfWork.GetEntityRepository<InterviewTemplateVersion>().DbSet
            .AsNoTracking()
            .AsSplitQuery()
            .Include(v => v.Axes).ThenInclude(a => a.InterviewEvaluationAxis)
            .Include(v => v.Axes).ThenInclude(a => a.Criteria).ThenInclude(c => c.InterviewEvaluationCriterion)
            .FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken);

        if (version is null)
            return Result.Fail<TemplateVersionDetailsDto>(new Error(ErrorsCodes.InterviewTemplateVersionNotFound));

        var axes = version.Axes
            .OrderBy(a => a.OrderNo)
            .Select(a => new TemplateVersionAxisDto(
                a.Id,
                a.InterviewTemplateVersionId,
                a.InterviewEvaluationAxisId,
                a.InterviewEvaluationAxis!.NameAr,
                a.InterviewEvaluationAxis.NameEn,
                a.MaxScore,
                a.QualificationScore,
                a.OrderNo,
                a.Criteria
                    .OrderBy(c => c.OrderNo)
                    .Select(c => new TemplateVersionCriterionDto(
                        c.Id,
                        c.InterviewTemplateEvaluationAxisId,
                        c.InterviewEvaluationCriterionId,
                        c.NameAr ?? c.InterviewEvaluationCriterion?.NameAr,
                        c.NameEn ?? c.InterviewEvaluationCriterion?.NameEn,
                        c.DescriptionAr ?? c.InterviewEvaluationCriterion?.DescriptionAr,
                        c.DescriptionEn ?? c.InterviewEvaluationCriterion?.DescriptionEn,
                        c.MaxScore,
                        c.IsRequired,
                        c.OrderNo,
                        c.Notes))
                    .ToList()))
            .ToList();

        var dto = new TemplateVersionDetailsDto(
            version.Id,
            version.InterviewTemplateId,
            version.VersionNo,
            version.FinalScore,
            version.QualificationScore,
            version.CalculationMethod,
            version.Status,
            version.IsLocked,
            version.EffectiveFrom,
            version.ApprovedById,
            version.ApprovedAt,
            version.DecisionNotes,
            axes);

        return Result.Ok(dto);
    }
}
