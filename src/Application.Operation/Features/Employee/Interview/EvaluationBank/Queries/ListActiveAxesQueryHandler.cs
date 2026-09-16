using System;
using System.Collections.Generic;
using System.Text;
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

public sealed class ListActiveAxesQueryHandler(IUnitOfWork uow) :
    IRequestHandler<ListActiveAxesQuery, IResult<PaginatedResult<AxisDto>>>
{
    public async Task<IResult<PaginatedResult<AxisDto>>> Handle(ListActiveAxesQuery request, CancellationToken cancellationToken)
    {
        var query = uow.GetEntityRepository<InterviewEvaluationAxis>().DbSet
            .AsNoTracking()
            .WhereIf(request.IsActive.HasValue, a => a.IsActive == request.IsActive!.Value)
            .WhereIf(!string.IsNullOrWhiteSpace(request.Search),
                a =>
                    EF.Functions.Like(a.NameAr, $"%{request.Search}%") ||
                    EF.Functions.Like(a.NameEn ?? "", $"%{request.Search}%") ||
                    EF.Functions.Like(a.DescriptionAr ?? "", $"%{request.Search}%") ||
                    EF.Functions.Like(a.DescriptionEn ?? "", $"%{request.Search}%"))
            .OrderBy(a => a.NameAr)
            .ThenBy(a => a.CreatedDate)
            .Select(a => new AxisDto(a.Id, a.NameAr, a.NameEn, a.DescriptionAr, a.DescriptionEn, a.IsActive));

        var result = await query.ToPaginatedListAsync(request, cancellationToken);

        return Result.Ok(result);
    }
}
