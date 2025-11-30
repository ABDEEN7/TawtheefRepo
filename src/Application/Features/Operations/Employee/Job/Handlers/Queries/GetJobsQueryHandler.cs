using FluentResults;
using MapsterMapper;
using MediatR;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Features.Operations.Employee.Job.Dtos;
using Tawtheef.Application.Features.Operations.Employee.Job.Queries;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Handlers.Queries;

public class GetJobsQueryHandler(IJobRepository jobRepository,IMapper mapper)
    : IRequestHandler<GetJobsQuery, IResult<PaginatedResult<JobResponseDto>>>
{
    public async Task<IResult<PaginatedResult<JobResponseDto>>> Handle(GetJobsQuery request, CancellationToken cancellationToken)
    {
        var result = await jobRepository.GetFilteredJobsAsync(
            filter: request.Filter ?? new JobQueryFilter(),
            pagination: request.Pagination ?? new PaginatedRequest()
        );
        
        if (result.IsFailed)
            return Result.Fail<PaginatedResult<JobResponseDto>>(result.Errors);

        var jobs = result.Value;
        
        // ✅ Use Mapster instead of manual mapping
        var dtos = mapper.Map<List<JobResponseDto>>(jobs.Items);    
        return Result.Ok(new PaginatedResult<JobResponseDto>(
            dtos, 
            jobs.Metadata.TotalCount, 
            jobs.Metadata.CurrentPage, 
            jobs.Metadata.PageSize
        ));
    }
}
