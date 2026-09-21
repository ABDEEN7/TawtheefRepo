using Application.Operation.Features.Employee.Interview.Committee.DTOs;
using Application.Operation.Features.Employee.Interview.Committee.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.Committee.Handlers.Queries;

public sealed class GetCommitteeByIdQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetCommitteeByIdQuery, IResult<CommitteeDto>>
{
    public async Task<IResult<CommitteeDto>> Handle(GetCommitteeByIdQuery request, CancellationToken cancellationToken)
    {
        var committee = await unitOfWork.GetEntityRepository<InterviewCommittee>().DbSet
            .AsNoTracking()
            .Where(c => c.Id == request.Id)
            .Select(c => new CommitteeDto(
                c.Id,
                c.JobId,
                c.Job != null && c.Job.JobTitle != null ? c.Job.JobTitle.JobNameAr : null,
                c.Job != null && c.Job.JobTitle != null ? c.Job.JobTitle.JobNameEn : null,
                c.InterviewTemplateId,
                c.InterviewTemplate!.TitleAr,
                c.InterviewTemplate.TitleEn,
                c.Job!.JobCategoryId,
                c.Job.JobCategory!.NameAr,
                c.Job.JobCategory.NameEn,
                c.NameAr,
                c.NameEn,
                c.ScopeDescription,
                c.Notes,
                c.Status,
                c.IsActive,
                c.ApprovedById,
                c.ApprovedAt,
                c.ClosedAt,
                c.DecisionNotes))
            .FirstOrDefaultAsync(cancellationToken);

        if (committee is null)
            return Result.Fail<CommitteeDto>(new Error(ErrorsCodes.InterviewCommitteeNotFound));

        return Result.Ok(committee);
    }
}
