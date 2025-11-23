using Mapster;
using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;
using Tawtheef.Application.Features.Operations.Employee.Job.Queries;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Handlers.Queries;

public class GetFilteredJobsQueryHandler(IJobRepository jobRepository)
    : IRequestHandler<GetFilteredJobsQuery, IResult<PaginatedResult<JobResponseDto>>>
{
    public async Task<IResult<PaginatedResult<JobResponseDto>>> Handle(GetFilteredJobsQuery request, CancellationToken cancellationToken)
    {
        var filter = new JobQueryFilter
        {
            SearchTerm = request.SearchTerm,
            DepartmentId = request.DepartmentId,
            StatusId = request.StatusId,
            JobCategoryId = request.JobCategoryId,
            WorkTypeId = request.WorkTypeId,
            DeadlineFrom = request.DeadlineFrom,
            DeadlineTo = request.DeadlineTo,
            MinVacancies = request.MinVacancies,
            MaxVacancies = request.MaxVacancies
        };

        var pagination = request.ToPaginatedRequest();
        var result = await jobRepository.GetFilteredJobsAsync(filter, pagination);
        
        if (result.IsFailed)
            return Result.Fail<PaginatedResult<JobResponseDto>>(result.Errors);

        var jobs = result.Value;
        
        // ✅ Use Mapster instead of manual mapping
        var dtos = jobs.Items.Adapt<List<JobResponseDto>>();
        
        return Result.Ok(new PaginatedResult<JobResponseDto>(
            dtos, 
            jobs.Metadata.TotalCount, 
            jobs.Metadata.CurrentPage, 
            jobs.Metadata.PageSize
        ));
    }
}
