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
        var templates = await unitOfWork.GetEntityRepository<InterviewTemplate>().DbSet
            .AsNoTracking()
            .OrderByDescending(t => t.CreatedDate)
            .Select(t => new TemplateDto(
                t.Id,
                t.TitleAr,
                t.TitleEn,
                t.OrganizationScopeId,
                t.OrganizationScope != null ? t.OrganizationScope.NameAr : null,
                t.OrganizationScope != null ? t.OrganizationScope.NameEn : null,
                t.JobTitleId,
                t.JobTitle != null ? t.JobTitle.JobNameAr : null,
                t.JobTitle != null ? t.JobTitle.JobNameEn : null,
                t.DepartmentId,
                t.Department != null ? t.Department.NameAr : null,
                t.Department != null ? t.Department.NameEn : null,
                t.IsActive))
            .ToListAsync(cancellationToken);

        return Result.Ok(templates);
    }
}
