using Application.Operation.Features.Employee.Interview.EvaluationTemplate.DTOs;
using Application.Operation.Features.Employee.Interview.EvaluationTemplate.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.EvaluationTemplate.Handlers.Queries;

public sealed class ListTemplatesQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ListTemplatesQuery, IResult<List<TemplateDto>>>
{
    public async Task<IResult<List<TemplateDto>>> Handle(ListTemplatesQuery request, CancellationToken cancellationToken)
    {
        var versions = unitOfWork.GetEntityRepository<InterviewTemplateVersion>().DbSet.AsNoTracking();

        var templates = await unitOfWork.GetEntityRepository<InterviewTemplate>().DbSet
            .AsNoTracking()
            .OrderByDescending(t => t.CreatedDate)
            .Select(t => new
            {
                Template = t,
                Latest = versions
                    .Where(v => v.InterviewTemplateId == t.Id)
                    .OrderByDescending(v => v.VersionNo)
                    .FirstOrDefault()
            })
            .Select(x => new TemplateDto(
                x.Template.Id,
                x.Template.TitleAr,
                x.Template.TitleEn,
                x.Template.OrganizationScopeId,
                x.Template.OrganizationScope != null ? x.Template.OrganizationScope.NameAr : null,
                x.Template.OrganizationScope != null ? x.Template.OrganizationScope.NameEn : null,
                x.Template.JobTitleId,
                x.Template.JobTitle != null ? x.Template.JobTitle.JobNameAr : null,
                x.Template.JobTitle != null ? x.Template.JobTitle.JobNameEn : null,
                x.Template.DepartmentId,
                x.Template.Department != null ? x.Template.Department.NameAr : null,
                x.Template.Department != null ? x.Template.Department.NameEn : null,
                x.Template.IsActive,
                x.Latest != null ? x.Latest.VersionNo : (int?)null,
                x.Latest != null ? x.Latest.Status : (TemplateVersionStatus?)null,
                x.Latest != null ? x.Latest.FinalScore : (decimal?)null,
                x.Latest != null ? x.Latest.QualificationScore : (decimal?)null))
            .ToListAsync(cancellationToken);

        return Result.Ok(templates);
    }
}

