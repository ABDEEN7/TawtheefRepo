using Application.Operation.Features.Employee.JobManagement.Job.DTOs;
using Application.Operation.Features.Employee.JobManagement.Job.Queries;
using MediatR;
using FluentResults;
using MapsterMapper;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Models.Pagination;
using Microsoft.AspNetCore.Http;
using Tawtheef.Application.Common.Interfaces.Services.Security;

namespace Application.Operation.Features.Employee.JobManagement.Job.Handlers.Queries;

public class GetJobsQueryHandler(
    IJobRepository jobRepository,
    IMapper mapper,
    ICurrentUserService currentUserService,
    IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<GetJobsQuery, IResult<PaginatedResult<JobResponseDto>>>
{
    public async Task<IResult<PaginatedResult<JobResponseDto>>> Handle(
    GetJobsQuery request, CancellationToken cancellationToken)
    {
        var isHrManager = httpContextAccessor.HttpContext?.User.IsInRole("HrManager") ?? false;
        Guid.TryParse(currentUserService.UserId, out var parsedUserId);
        var result = await jobRepository.GetFilteredJobsAsync(
            filter: request.Filter ?? new JobQueryFilter(),
            pagination: request.Pagination,
            currentUserId: parsedUserId,
            isHRManager: isHrManager
        );

        if (result.IsFailed)
            return Result.Fail<PaginatedResult<JobResponseDto>>(result.Errors);

        var jobs = result.Value;

        
        var dtoItems = mapper.Map<List<JobResponseDto>>(jobs.Items);

        // Enhance with calculated fields
        for (int i = 0; i < jobs.Items.Count; i++)
        {
            var job = jobs.Items[i];
            var dto = dtoItems[i];
            
            dto.CreatedByName = job.CreatedBy?.FullNameEn;
            dto.LastActionDate = job.UpdatedDate ?? job.CreatedDate;
        }

        var paginatedDto = new PaginatedResult<JobResponseDto>(
            dtoItems,
            jobs.Metadata.TotalCount,
            jobs.Metadata.CurrentPage,
            jobs.Metadata.PageSize
        );

        return Result.Ok(paginatedDto);
    }
}

