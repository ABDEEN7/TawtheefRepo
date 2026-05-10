using Application.Operation.Features.Employee.JobManagement.JobOperations.DTOs;
using Application.Operation.Features.Employee.JobManagement.JobOperations.Queries;
using MediatR;
using FluentResults;
using MapsterMapper;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Models.Pagination;
using Microsoft.AspNetCore.Http;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Application.Common.Security;


namespace Application.Operation.Features.Employee.JobManagement.JobOperations.Handlers.Queries;

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
        var user = httpContextAccessor.HttpContext?.User;
        var canViewAllJobs = user?.HasFullJobAccess() ?? false;

        Guid.TryParse(currentUserService.UserId, out var parsedUserId);
        var result = await jobRepository.GetFilteredJobsAsync(
            filter: request.Filter ?? new JobQueryFilter(),
            pagination: request.Pagination,
            currentUserId: parsedUserId,
            isHRManager: canViewAllJobs
        );

        if (result.IsFailed)
            return Result.Fail<PaginatedResult<JobResponseDto>>(result.Errors);

        var jobs = result.Value;

        
        var dtoItems = mapper.From(jobs.Items)
            .AddParameters("IsHrManager", canViewAllJobs)
            .AddParameters("CurrentUserId", parsedUserId)
            .AdaptToType<List<JobResponseDto>>();

        var paginatedDto = new PaginatedResult<JobResponseDto>(
            dtoItems,
            jobs.Metadata.TotalCount,
            jobs.Metadata.CurrentPage,
            jobs.Metadata.PageSize
        );

        return Result.Ok(paginatedDto);
    }
}

