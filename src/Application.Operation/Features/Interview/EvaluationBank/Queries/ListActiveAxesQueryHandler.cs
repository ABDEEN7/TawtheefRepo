using System;
using System.Collections.Generic;
using System.Text;
using Application.Operation.Features.Interview.EvaluationBank.DTOs;
using Application.Operation.Features.Interview.EvaluationBank.Handlers.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Interview.EvaluationBank.Queries;

public sealed class ListActiveAxesQueryHandler(IUnitOfWork uow) :
    IRequestHandler<ListActiveAxesQuery, IResult<List<AxisDto>>>
{
    public async Task<IResult<List<AxisDto>>> Handle(ListActiveAxesQuery request, CancellationToken cancellationToken)
    {
        // if we need to list all axis even unactive one and filter it on front end we can remove Where(a => a.IsActive) condetion : will discuss woth Bussiness.
        var axes = await uow.GetEntityRepository<InterviewEvaluationAxis>().DbSet
        .AsNoTracking()
        .Where(a => a.IsActive)
        .OrderBy(a => a.NameAr)
        .ThenBy(a => a.CreatedDate)
        .Select(a => new AxisDto(a.Id, a.NameAr, a.NameEn, a.DescriptionAr, a.DescriptionEn, a.IsActive))
        .ToListAsync(cancellationToken);

        return Result.Ok(axes);
    }
}
