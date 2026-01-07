using Cortex.Mediator.Queries;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.DTOs;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.Queries;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.Services;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Application.Features.Operations.Employee.JobCandidates.Handlers.Queries;

public sealed class GetJobCandidatesQueryHandler(
    IUnitOfWork unitOfWork,
    ILocalizationService localizationService)
    : IQueryHandler<GetJobCandidatesQuery, IResult<JobCandidatesCombinedDto>>
{
    private readonly JobCandidateScoringService _scoring = new();

    public async Task<IResult<JobCandidatesCombinedDto>> Handle(
        GetJobCandidatesQuery request,
        CancellationToken ct)
    {
        var job = await LoadJobAsync(request.JobId, ct);
        if (job is null)
            return Result.Fail<JobCandidatesCombinedDto>(JobMessages.JobNotFound);

        var targetCount = GetTargetCount(job);
        var req = await JobRequirementsService.GetAsync(unitOfWork, job, ct);

        var baseQuery = JobCandidatesQueryBuilder.BuildEligibleQuery(
            unitOfWork, job, req, request.Filter);

        var pageNumber = request.Pagination.PageNumber;
        var pageSize = request.Pagination.PageSize;

        // window = enough candidates to score and then apply filters before paging
        var windowSize = Math.Max(targetCount * 10, pageSize * 10);

        var window = await baseQuery
            .OrderByDescending(x => x.CreatedDate)
            .Take(windowSize)
            .ToListAsync(ct);

        if (window.Count == 0)
        {
            return Result.Ok(new JobCandidatesCombinedDto
            {
                List = new PaginatedResult<JobCandidateListItemDto>([], 0, pageNumber, pageSize),
                Overview = new JobCandidatesOverviewDto
                {
                    TotalCandidatesCount = await unitOfWork.GetEntityRepository<Invitation>().DbSet
                        .CountAsync(i => i.JobId == job.Id, cancellationToken: ct),
                    AvailableCandidatesCount = targetCount,
                    AbovePointsCandidatesCount = 0,
                    PointsAverage = 0
                }
            });
        }

        // Load heavy profiles once
        var ids = window.Select(x => x.ApplicantId).Distinct().ToList();
        var profiles = await CandidateProfileLoader.LoadForScoringAsync(unitOfWork, ids, ct);

        // Score once
        var scored = _scoring.Score(window, profiles, job, req);

        // Apply minimum points (common filter)
        if (request.Filter?.MinimumPoints is not null)
            scored = scored.Where(x => x.Points >= request.Filter.MinimumPoints.Value).ToList();

        // Sort by points
        var sorted = scored
            .OrderByDescending(x => x.Points)
            .ThenByDescending(x => x.CreatedDate)
            .ToList();

        // Load percentage filter settings (used for the list)
        var settings = await unitOfWork.GetEntityRepository<JobCandidateFilterSetting>().DbSet
            .AsNoTracking()
            .Include(s => s.CandidateTypePercentages)
            .Include(s => s.NationalityPercentages)
            .FirstOrDefaultAsync(s => s.JobId == request.JobId, ct);

        var finalList = JobCandidatesFilterProcessor.ApplyPercentageFilters(sorted, settings, targetCount);

        var totalInvited = await unitOfWork.GetEntityRepository<Invitation>().DbSet
            .CountAsync(i => i.JobId == job.Id, cancellationToken: ct);

        var totalEligible = finalList.Count;
        var abovePoints = finalList.Count(x => x.Points >= 800);
        var avg = totalEligible == 0 ? 0 : finalList.Average(x => x.Points);

        var overview = new JobCandidatesOverviewDto
        {
            TotalCandidatesCount = totalInvited,
            AvailableCandidatesCount = totalEligible,
            AbovePointsCandidatesCount = abovePoints,
            PointsAverage = Math.Round(avg, 2)
        };

        // ===== List paging + mapping =====
        var paged = finalList
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var items = paged.Select(candidate => new JobCandidateListItemDto
        {
            InvitationId = candidate.InvitationId,
            CandidateId = candidate.ApplicantId,
            CandidateName = localizationService.GetLocalizedFullName(candidate.Applicant),
            Department = localizationService.GetLocalizedName(job.Department),
            JobCategory = localizationService.GetLocalizedName(job.JobCategory),
            CandidateCategory = localizationService.GetLocalizedName(candidate.Profile?.CandidateType),
            CandidateMajor = localizationService.GetLocalizedName(candidate.Major),
            CandidateGender = localizationService.GetLocalizedName(candidate.Profile?.Gender),
            Points = candidate.Points
        }).ToList();

        var list = new PaginatedResult<JobCandidateListItemDto>(items, finalList.Count, pageNumber, pageSize);

        return Result.Ok(new JobCandidatesCombinedDto
        {
            List = list,
            Overview = overview
        });
    }

    private async Task<Domain.Entities.Recruitment.Job?> LoadJobAsync(Guid jobId, CancellationToken ct)
    {
        return await unitOfWork.GetEntityRepository<Domain.Entities.Recruitment.Job>().DbSet
            .AsNoTracking()
            .Include(j => j.JobPoints).ThenInclude(p => p!.Details)
            .Include(j => j.Department)
            .Include(j => j.JobCategory)
            .Include(j => j.Major)
            .Include(j => j.SubMajor)
            .FirstOrDefaultAsync(j => j.Id == jobId, ct);
    }

    private static int GetTargetCount(Domain.Entities.Recruitment.Job job)
    {
        var vacancies = Math.Max(1, job.NumberOfVacancies);
        return job.JobCategoryId == JobCategoryIds.Academic ? vacancies * 5 : vacancies;
    }
}
