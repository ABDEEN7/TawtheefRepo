using Application.Operation.Features.Employee.JobManagement.Job.DTOs;
using Application.Operation.Features.Employee.JobManagement.Job.Queries;
using MediatR;
using FluentResults;
using MapsterMapper;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Models.Pagination;

namespace Application.Operation.Features.Employee.JobManagement.Job.Handlers.Queries;

public class GetJobsQueryHandler(IJobRepository jobRepository,IMapper mapper)
    : IRequestHandler<GetJobsQuery, IResult<PaginatedResult<JobResponseDto>>>
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

