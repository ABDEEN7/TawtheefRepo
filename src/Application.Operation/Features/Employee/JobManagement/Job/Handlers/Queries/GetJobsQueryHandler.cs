using Application.Operation.Features.Employee.Job.DTOs;
using Application.Operation.Features.Employee.Job.Queries;
using Cortex.Mediator.Queries;
using FluentResults;
using MapsterMapper;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Employee.Job.Handlers.Queries;

public class GetJobsQueryHandler(IJobRepository jobRepository,IMapper mapper)
    : IQueryHandler<GetJobsQuery, IResult<PaginatedResult<JobResponseDto>>>
{
    public async Task<IResult<PaginatedResult<JobResponseDto>>> Handle(
    GetJobsQuery request, CancellationToken cancellationToken)
    {
        var result = await jobRepository.GetFilteredJobsAsync(
            filter: request.Filter ?? new JobQueryFilter(),
            pagination: request.Pagination
        );

        if (result.IsFailed)
            return Result.Fail<PaginatedResult<JobResponseDto>>(result.Errors);

        var jobs = result.Value;

        
        var dtoItems = mapper.Map<List<JobResponseDto>>(jobs.Items);

        var paginatedDto = new PaginatedResult<JobResponseDto>(
            dtoItems,
            jobs.Metadata.TotalCount,
            jobs.Metadata.CurrentPage,
            jobs.Metadata.PageSize
        );

        return Result.Ok(paginatedDto);
    }
}
