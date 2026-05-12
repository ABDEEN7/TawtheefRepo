using Application.Operation.Features.Employee.JobManagement.JobOperations.DTOs;
using Application.Operation.Features.Employee.JobManagement.JobOperations.Queries;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.Services;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.Services.Interfaces;
using MediatR;
using FluentResults;
using MapsterMapper;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models.Pagination;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Application.Common.Security;
using Tawtheef.Domain.Entities.Recruitment;


namespace Application.Operation.Features.Employee.JobManagement.JobOperations.Handlers.Queries;

public class GetJobsQueryHandler(
    IJobRepository jobRepository,
    IMapper mapper,
    IUnitOfWork unitOfWork,
    IJobTargetCandidateCalculatorService targetCandidateCalculator,
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

        await SetAvailableVacanciesAsync(jobs.Items, dtoItems, cancellationToken);

        var paginatedDto = new PaginatedResult<JobResponseDto>(
            dtoItems,
            jobs.Metadata.TotalCount,
            jobs.Metadata.CurrentPage,
            jobs.Metadata.PageSize
        );

        return Result.Ok(paginatedDto);
    }

    private async Task SetAvailableVacanciesAsync(
        IReadOnlyList<Tawtheef.Domain.Entities.Recruitment.Job> jobs,
        List<JobResponseDto> dtoItems,
        CancellationToken cancellationToken)
    {
        if (jobs.Count == 0)
            return;

        var jobIds = jobs.Select(job => job.Id).ToList();
        var activeInvitationCounts = await unitOfWork
            .GetEntityRepository<Invitation>()
            .DbSet
            .AsNoTracking()
            .Where(invitation =>
                jobIds.Contains(invitation.JobId) &&
                CandidateEligibilityRules.ActiveInvitationStatuses.Contains(invitation.InvitationStatusId))
            .GroupBy(invitation => invitation.JobId)
            .Select(group => new { JobId = group.Key, Count = group.Count() })
            .ToDictionaryAsync(group => group.JobId, group => group.Count, cancellationToken);

        var dtoMap = dtoItems.ToDictionary(job => job.Id);

        foreach (var job in jobs)
        {
            var targetCount = await targetCandidateCalculator.GetTargetCountAsync(
                job.JobCategoryId,
                job.NumberOfVacancies);
            var activeInvitationCount = activeInvitationCounts.GetValueOrDefault(job.Id);

            dtoMap[job.Id].AvailableVacancies = Math.Max(targetCount - activeInvitationCount, 0);
        }
    }
}

