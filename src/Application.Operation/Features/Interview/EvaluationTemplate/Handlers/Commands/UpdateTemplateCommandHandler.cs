using Application.Operation.Features.Interview.EvaluationTemplate.Commands;
using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Application.Operation.Features.Interview.EvaluationTemplate.Handlers.Commands;

public sealed class UpdateTemplateCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateTemplateCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(UpdateTemplateCommand request, CancellationToken cancellationToken)
    {
        var repo = unitOfWork.GetEntityRepository<InterviewTemplate>().DbSet;
        var template = await repo.FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);
        if (template is null)
            return Result.Fail<Unit>(new Error(ErrorsCodes.InterviewTemplateNotFound));

        if (request.OrganizationScopeId.HasValue)
        {
            var organizationScopeExists = await unitOfWork.GetEntityRepository<InterviewOrganizationScope>().DbSet
                .AnyAsync(s => s.Id == request.OrganizationScopeId.Value, cancellationToken);
            if (!organizationScopeExists)
                return Result.Fail<Unit>(new Error(ErrorsCodes.InterviewOrganizationScopeNotFound));
        }

        if (request.JobTitleId.HasValue)
        {
            var jobTitleExists = await unitOfWork.GetEntityRepository<JobTitle>().DbSet
                .AnyAsync(j => j.Id == request.JobTitleId.Value, cancellationToken);
            if (!jobTitleExists)
                return Result.Fail<Unit>(new Error(ErrorsCodes.JobTitleNotFound));
        }

        if (request.DepartmentId.HasValue)
        {
            var departmentExists = await unitOfWork.GetEntityRepository<Department>().DbSet
                .AnyAsync(d => d.Id == request.DepartmentId.Value, cancellationToken);
            if (!departmentExists)
                return Result.Fail<Unit>(new Error(ErrorsCodes.DepartmentNotFound));
        }

        template.TitleAr = request.TitleAr;
        template.TitleEn = request.TitleEn;
        template.OrganizationScopeId = request.OrganizationScopeId;
        template.JobTitleId = request.JobTitleId;
        template.DepartmentId = request.DepartmentId;
        template.IsActive = request.IsActive;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(Unit.Value);
    }
}
