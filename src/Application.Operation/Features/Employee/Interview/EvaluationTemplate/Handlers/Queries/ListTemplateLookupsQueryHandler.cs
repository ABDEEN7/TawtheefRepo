using System;
using System.Collections.Generic;
using System.Text;
using FluentResults;
using MediatR;
using Application.Operation.Features.Employee.Interview.EvaluationTemplate.DTOs;
using Application.Operation.Features.Employee.Interview.EvaluationTemplate.Queries;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Application.Operation.Features.Employee.Interview.EvaluationTemplate.Handlers.Queries;

public sealed class ListTemplateLookupsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ListTemplateLookupsQuery, IResult<TemplateLookupsDto>>
{
    public async Task<IResult<TemplateLookupsDto>> Handle(ListTemplateLookupsQuery request, CancellationToken cancellationToken)
    {
        var organizationScopes = await unitOfWork.GetEntityRepository<InterviewOrganizationScope>().DbSet
            .AsNoTracking()
            .Where(s => s.IsActive)
            .OrderBy(s => s.DisplayOrder)
            .Select(s => new TemplateLookupItemDto(s.Id, s.NameAr, s.NameEn))
            .ToListAsync(cancellationToken);

        var jobTitles = await unitOfWork.GetEntityRepository<JobTitle>().DbSet
            .AsNoTracking()
            .Where(j => j.IsActive)
            .OrderBy(j => j.JobNameAr)
            .Select(j => new TemplateLookupItemDto(j.Id, j.JobNameAr, j.JobNameEn))
            .ToListAsync(cancellationToken);

        var departments = await unitOfWork.GetEntityRepository<Department>().DbSet
            .AsNoTracking()
            .Where(d => d.IsActive)
            .OrderBy(d => d.NameAr)
            .Select(d => new TemplateLookupItemDto(d.Id, d.NameAr, d.NameEn))
            .ToListAsync(cancellationToken);

        return Result.Ok(new TemplateLookupsDto(organizationScopes, jobTitles, departments));
    }
}
