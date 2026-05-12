using Application.Operation.Common.Repositories;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.DTOs;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.Models;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.Queries;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.Services;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.Services.Interfaces;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.Utilities;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Logging;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Handlers.Queries;

public sealed class GetJobCandidatesQueryHandler(
    IUnitOfWork unitOfWork,
    IUserProfileRepository userProfileRepository,
    IJobRepository jobRepository,
    IJobTargetCandidateCalculatorService targetCandidateCalculator,
    IJobRequirementsService jobRequirementsService,
    IJobCandidatesQueryBuilderService queryBuilderService,
    ILocalizationService localizationService,
    IAppLogger logger)
    : IRequestHandler<GetJobCandidatesQuery, IResult<JobCandidatesCombinedDto>>
{
    public async Task<IResult<JobCandidatesCombinedDto>> Handle(
        GetJobCandidatesQuery request,
        CancellationToken ct)
    {
        var job = await jobRepository.LoadJobWithPointsAsync(request.JobId);

        if (job is null)
            return Result.Fail<JobCandidatesCombinedDto>(JobMessages.JobNotFound);

        if (job.JobPoints is null)
            return Result.Fail<JobCandidatesCombinedDto>(JobMessages.JobPointsNotFound);

        var pageNumber = request.PageNumber;
        var pageSize = request.PageSize;

        var targetCount = await targetCandidateCalculator.GetTargetCountAsync(
            job.JobCategoryId,
            job.NumberOfVacancies);
        var activeInvitationCount = await CountActiveInvitationsAsync(job.Id, ct);
        var availableVacancies = Math.Max(targetCount - activeInvitationCount, 0);

        var requirements = await jobRequirementsService.GetAsync(job);

        var candidatesQuery = queryBuilderService.BuildEligibleQuery(
            job.Id,
            job.WorkLocationId,
            job.GenderId,
            job.MaximumAge,
            job.MinimumAge,
            requirements,
            request.Filter);

        var candidatesWindow = await LoadCandidateWindowAsync(
            candidatesQuery,
            targetCount,
            pageSize,
            ct);

        if (candidatesWindow.Count == 0)
            return Result.Ok(CreateEmptyResponse(pageNumber, pageSize, availableVacancies));

        var scoredCandidates = await ScoreCandidatesAsync(
            candidatesWindow,
            job,
            request,
            ct);

        var sortedCandidates = scoredCandidates
            .OrderByDescending(candidate => candidate.Points)
            .ToList();

        var filterSettings = await LoadFilterSettingsAsync(request.JobId, ct);

        var finalCandidates = JobCandidatesFilterUtility.ApplyPercentageFilters(
            sortedCandidates,
            filterSettings,
            targetCount);

        var overview = CreateOverview(scoredCandidates, finalCandidates, availableVacancies);

        var list = CreatePaginatedList(finalCandidates, job,
            new PaginatedRequest { PageNumber = pageNumber, PageSize = pageSize });

        return Result.Ok(new JobCandidatesCombinedDto
        {
            List = list,
            Overview = overview
        });
    }

    private static async Task<List<JobCandidateRecord>> LoadCandidateWindowAsync(
        IQueryable<JobCandidateRecord> query,
        int targetCount,
        int pageSize,
        CancellationToken ct)
    {
        var windowSize = Math.Max(targetCount * 10, pageSize * 10);

        return await query
            .OrderByDescending(candidate => candidate.CreatedDate)
            .Take(windowSize)
            .ToListAsync(ct);
    }

    private async Task<List<JobCandidateRecord>> ScoreCandidatesAsync(
        List<JobCandidateRecord> candidates,
        Job job,
        GetJobCandidatesQuery request,
        CancellationToken ct)
    {
        var applicantIds = candidates
            .Select(candidate => candidate.ApplicantId)
            .Distinct()
            .ToList();

        var profiles = await userProfileRepository.LoadForScoringAsync(applicantIds, ct);

        var scoredCandidates = JobCandidateScoringUtility.Score(
            candidates,
            profiles,
            job.JobPoints!,
            job.MajorId,
            job.SubMajorId,
            job.JobDegrees,
            logger);

        if (request.Filter?.MinimumPoints is null)
            return scoredCandidates;

        return scoredCandidates
            .Where(candidate => candidate.Points >= request.Filter.MinimumPoints.Value)
            .ToList();
    }

    private async Task<JobCandidateFilterSetting?> LoadFilterSettingsAsync(
        Guid jobId,
        CancellationToken ct)
    {
        return await unitOfWork
            .GetEntityRepository<JobCandidateFilterSetting>()
            .DbSet
            .AsNoTracking()
            .AsSplitQuery()
            .Include(setting => setting.CandidateTypePercentages)
            .Include(setting => setting.NationalityPercentages)
            .FirstOrDefaultAsync(setting => setting.JobId == jobId, ct);
    }

    private async Task<int> CountActiveInvitationsAsync(
        Guid jobId,
        CancellationToken ct)
    {
        return await unitOfWork
            .GetEntityRepository<Invitation>()
            .DbSet
            .AsNoTracking()
            .CountAsync(invitation =>
                invitation.JobId == jobId &&
                CandidateEligibilityRules.ActiveInvitationStatuses.Contains(invitation.InvitationStatusId),
                ct);
    }

    private static JobCandidatesOverviewDto CreateOverview(
        List<JobCandidateRecord> scoredCandidates,
        List<JobCandidateRecord> finalCandidates,
        int availableVacancies)
    {
        var totalEligible = scoredCandidates.Count;
        var pointsAverage = totalEligible == 0
            ? 0
            : scoredCandidates.Average(candidate => candidate.Points);

        return new JobCandidatesOverviewDto
        {
            TotalCandidatesCount = totalEligible,
            AvailableCandidatesCount = finalCandidates.Count,
            AvailableVacancies = availableVacancies,
            PointsAverage = Math.Round(pointsAverage, 2)
        };
    }

    private PaginatedResult<JobCandidateListItemDto> CreatePaginatedList(
        List<JobCandidateRecord> candidates, Job job, PaginatedRequest paginatedRequest)
    {
        var pageResult = candidates
            .Select(candidate => new JobCandidateListItemDto
            {
                InvitationId = candidate.InvitationId,
                CandidateId = candidate.ApplicantId,
                CandidateName = localizationService.GetLocalizedFullName(candidate.Applicant),
                Department = localizationService.GetLocalizedName(job.Department),
                JobCategory = localizationService.GetLocalizedName(job.JobCategory),
                CandidateCategory = localizationService.GetLocalizedName(candidate.Profile?.CandidateType),
                CandidateGender = localizationService.GetLocalizedName(candidate.Profile?.Gender),
                GroupOrder = candidate.GroupOrder,
                Points = candidate.Points
            })
            .OrderByDescending(candidate => candidate.GroupOrder)
            .ThenByDescending(candidate => candidate.Points)
            .ToPaginatedList(paginatedRequest);

        return pageResult;
    }

    private static JobCandidatesCombinedDto CreateEmptyResponse(
        int pageNumber,
        int pageSize,
        int availableVacancies)
    {
        return new JobCandidatesCombinedDto
        {
            List = new PaginatedResult<JobCandidateListItemDto>(
                [],
                0,
                pageNumber,
                pageSize),
            Overview = new JobCandidatesOverviewDto
            {
                TotalCandidatesCount = 0,
                AvailableCandidatesCount = 0,
                AvailableVacancies = availableVacancies,
                PointsAverage = 0
            }
        };
    }
}
