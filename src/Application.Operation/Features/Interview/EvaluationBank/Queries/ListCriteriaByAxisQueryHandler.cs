using Application.Operation.Features.Interview.EvaluationBank.DTOs;
using Application.Operation.Features.Interview.EvaluationBank.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Interview.EvaluationBank.Handlers.Queries;

public sealed class ListCriteriaByAxisQueryHandler(IUnitOfWork uow)
    : IRequestHandler<ListCriteriaByAxisQuery, IResult<List<CriterionDto>>>
{
    public async Task<IResult<List<CriterionDto>>> Handle(ListCriteriaByAxisQuery request, CancellationToken cancellationToken)
    {
        var criteria = await uow.GetEntityRepository<InterviewEvaluationCriterion>().DbSet
            .AsNoTracking()
            .Where(c => c.InterviewEvaluationAxisId == request.InterviewEvaluationAxisId && c.IsActive)
            .OrderByDescending(c => c.CreatedDate)
            .ThenBy(c => c.NameAr)
            .Select(c => new CriterionDto(
                c.Id, c.InterviewEvaluationAxisId, c.NameAr, c.NameEn, c.DescriptionAr, c.DescriptionEn, c.IsActive))
            .ToListAsync(cancellationToken);

        return Result.Ok(criteria);
    }
}
