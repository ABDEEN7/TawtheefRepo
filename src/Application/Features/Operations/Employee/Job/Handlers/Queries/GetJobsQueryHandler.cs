using FluentResults;
using Mapster;
using MediatR;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Mappers;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;
using Tawtheef.Application.Features.Operations.Employee.Job.Queries;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Handlers.Queries;

public class GetJobsQueryHandler(IJobRepository jobRepository)
    : IRequestHandler<GetJobsQuery, IResult<PaginatedResult<JobResponseDto>>>
{
    public async Task<IResult<PaginatedResult<JobResponseDto>>> Handle(
        GetJobsQuery request, CancellationToken cancellationToken)
    {
        var result = await jobRepository.GetFilteredJobsAsync(
            filter: request.Filter ?? new JobQueryFilter(),
            pagination: request.Pagination ?? new PaginatedRequest()
        );

        if (result.IsFailed)
            return Result.Fail<PaginatedResult<JobResponseDto>>(result.Errors);

        var jobs = result.Value;

        if (!jobs.Items.Any())
        {
            return Result.Ok(new PaginatedResult<JobResponseDto>(
                [],
                jobs.Metadata.TotalCount,
                jobs.Metadata.CurrentPage,
                jobs.Metadata.PageSize
            ));
        }


        var dtos = jobs.Items.Select(JobManualMapper.Map).ToList();


        return Result.Ok(new PaginatedResult<JobResponseDto>(
            dtos,
            jobs.Metadata.TotalCount,
            jobs.Metadata.CurrentPage,
            jobs.Metadata.PageSize
        ));
    }
}
