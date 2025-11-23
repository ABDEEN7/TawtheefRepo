using Mapster;
using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;
using Tawtheef.Application.Features.Operations.Employee.Job.Queries;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Handlers.Queries;

public class GetJobsQueryHandler(IJobRepository jobRepository)
    : IRequestHandler<GetJobsQuery, IResult<PaginatedResult<JobResponseDto>>>
{
    public async Task<IResult<PaginatedResult<JobResponseDto>>> Handle(GetJobsQuery request, CancellationToken cancellationToken)
    {
        var paginatedRequest = request.ToPaginatedRequest();
        var result = await jobRepository.GetPaginatedJobsAsync(paginatedRequest);
        
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
