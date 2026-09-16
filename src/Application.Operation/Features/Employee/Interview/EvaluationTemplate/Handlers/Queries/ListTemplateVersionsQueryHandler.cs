using Application.Operation.Features.Employee.Interview.EvaluationTemplate.DTOs;
using Application.Operation.Features.Employee.Interview.EvaluationTemplate.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.EvaluationTemplate.Handlers.Queries;

public sealed class ListTemplateVersionsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ListTemplateVersionsQuery, IResult<List<TemplateVersionDto>>>
{
    public async Task<IResult<List<TemplateVersionDto>>> Handle(ListTemplateVersionsQuery request, CancellationToken cancellationToken)
    {
        var versions = await unitOfWork.GetEntityRepository<InterviewTemplateVersion>().DbSet
            .AsNoTracking()
            .Where(v => v.InterviewTemplateId == request.InterviewTemplateId)
            .OrderByDescending(v => v.VersionNo)
            .Select(v => new TemplateVersionDto(
                v.Id,
                v.InterviewTemplateId,
                v.VersionNo,
                v.FinalScore,
                v.QualificationScore,
                v.CalculationMethod,
                v.Status,
                v.IsLocked,
                v.EffectiveFrom,
                v.ApprovedById,
                v.ApprovedAt,
                v.DecisionNotes))
            .ToListAsync(cancellationToken);

        return Result.Ok(versions);
    }
}
