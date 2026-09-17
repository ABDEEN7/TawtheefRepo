using Application.Operation.Features.Employee.Interview.EvaluationBank.DTOs;
using Application.Operation.Features.Employee.Interview.EvaluationBank.Handlers.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.EvaluationBank.Queries;

public sealed class ListCriteriaByAxisQueryHandler(IUnitOfWork uow)
    : IRequestHandler<ListCriteriaByAxisQuery, IResult<PaginatedResult<CriterionDto>>>
{
    public async Task<IResult<PaginatedResult<CriterionDto>>> Handle(ListCriteriaByAxisQuery request, CancellationToken cancellationToken)
    {
        var query = uow.GetEntityRepository<InterviewEvaluationCriterion>().DbSet
            .AsNoTracking()
            .Where(c => c.InterviewEvaluationAxisId == request.InterviewEvaluationAxisId)
            .WhereIf(request.IsActive.HasValue, c => c.IsActive == request.IsActive!.Value)
            .WhereIf(!string.IsNullOrWhiteSpace(request.Search),
                c =>
                    EF.Functions.Like(c.NameAr, $"%{request.Search}%") ||
                    EF.Functions.Like(c.NameEn ?? "", $"%{request.Search}%") ||
                    EF.Functions.Like(c.DescriptionAr ?? "", $"%{request.Search}%") ||
                    EF.Functions.Like(c.DescriptionEn ?? "", $"%{request.Search}%"))
            .OrderByDescending(c => c.CreatedDate)
            .ThenBy(c => c.NameAr)
            .Select(c => new CriterionDto(
                c.Id, c.InterviewEvaluationAxisId, c.NameAr, c.NameEn, c.DescriptionAr, c.DescriptionEn, c.IsActive));

        var result = await query.ToPaginatedListAsync(request, cancellationToken);

        return Result.Ok(result);
    }
}
