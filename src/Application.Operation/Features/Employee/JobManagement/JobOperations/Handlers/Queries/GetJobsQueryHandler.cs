using Application.Operation.Features.Employee.Common.Access;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.Services;
using Application.Operation.Features.Employee.JobManagement.JobOperations.DTOs;
using Application.Operation.Features.Employee.JobManagement.JobOperations.Queries;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.Services.Interfaces;
using MediatR;
using FluentResults;
using MapsterMapper;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models.Pagination;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Entities.Recruitment;


namespace Application.Operation.Features.Employee.JobManagement.JobOperations.Handlers.Queries;

internal sealed class GetJobsQueryHandler(
    IJobRepository jobRepository,
    IMapper mapper,
    IUnitOfWork unitOfWork,
    IJobTargetCandidateCalculatorService targetCandidateCalculator,
    EmployeeJobAccessContextProvider accessProvider)
    : IRequestHandler<GetJobsQuery, IResult<PaginatedResult<JobResponseDto>>>
{
    public async Task<IResult<PaginatedResult<JobResponseDto>>> Handle(
    GetJobsQuery request, CancellationToken cancellationToken)
    {
        var access = accessProvider.GetAccess();
        var result = await jobRepository.GetFilteredJobsAsync(
            filter: request.Filter ?? new JobQueryFilter(),
            pagination: request.Pagination,
            currentUserId: access.CurrentUserId,
            hasFullAccess: access.HasFullAccess
        );

        if (result.IsFailed)
            return Result.Fail<PaginatedResult<JobResponseDto>>(result.Errors);

        var jobs = result.Value;

        
        var dtoItems = mapper.From(jobs.Items)
            .AddParameters("IsHrManager", access.HasFullAccess)
            .AddParameters("CurrentUserId", access.CurrentUserId ?? Guid.Empty)
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
        IReadOnlyList<Job> jobs,
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

        var targetCounts = await targetCandidateCalculator.GetTargetCountsAsync(
            jobs,
            cancellationToken);
        var dtoMap = dtoItems.ToDictionary(job => job.Id);

        foreach (var job in jobs)
        {
            var targetCount = targetCounts[job.Id];
            var activeInvitationCount = activeInvitationCounts.GetValueOrDefault(job.Id);

            dtoMap[job.Id].AvailableVacancies = Math.Max(targetCount - activeInvitationCount, 0);
        }
    }
}
