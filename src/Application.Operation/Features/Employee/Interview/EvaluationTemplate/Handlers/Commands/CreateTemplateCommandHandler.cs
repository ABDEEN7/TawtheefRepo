using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Application.Operation.Features.Employee.Interview.EvaluationTemplate.Commands;

namespace Application.Operation.Features.Employee.Interview.EvaluationTemplate.Handlers.Commands;

public sealed class CreateTemplateCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateTemplateCommand, IResult<Guid>>
{
    public async Task<IResult<Guid>> Handle(CreateTemplateCommand request, CancellationToken cancellationToken)
    {
        if (request.OrganizationScopeId.HasValue)
        {
            var organizationScopeExists = await unitOfWork.GetEntityRepository<InterviewOrganizationScope>().DbSet
                .AnyAsync(s => s.Id == request.OrganizationScopeId.Value, cancellationToken);
            if (!organizationScopeExists)
                return Result.Fail<Guid>(new Error(ErrorsCodes.InterviewOrganizationScopeNotFound));
        }

        if (request.JobTitleId.HasValue)
        {
            var jobTitleExists = await unitOfWork.GetEntityRepository<JobTitle>().DbSet
                .AnyAsync(j => j.Id == request.JobTitleId.Value, cancellationToken);
            if (!jobTitleExists)
                return Result.Fail<Guid>(new Error(ErrorsCodes.JobTitleNotFound));
        }

        if (request.DepartmentId.HasValue)
        {
            var departmentExists = await unitOfWork.GetEntityRepository<Department>().DbSet
                .AnyAsync(d => d.Id == request.DepartmentId.Value, cancellationToken);
            if (!departmentExists)
                return Result.Fail<Guid>(new Error(ErrorsCodes.DepartmentNotFound));
        }

        var template = new InterviewTemplate
        {
            TitleAr = request.TitleAr,
            TitleEn = request.TitleEn,
            OrganizationScopeId = request.OrganizationScopeId,
            JobTitleId = request.JobTitleId,
            DepartmentId = request.DepartmentId,
            IsActive = request.IsActive
        };

        await unitOfWork.GetEntityRepository<InterviewTemplate>().AddAsync(template);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(template.Id);
    }
}
