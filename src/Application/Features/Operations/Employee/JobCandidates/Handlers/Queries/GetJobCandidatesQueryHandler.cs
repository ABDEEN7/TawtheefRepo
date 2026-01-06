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
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Application.Features.Operations.Employee.JobCandidates.Handlers.Queries;

public sealed class GetJobCandidatesQueryHandler(
    IUnitOfWork unitOfWork,
    ILocalizationService localizationService)
    : IQueryHandler<GetJobCandidatesQuery, IResult<PaginatedResult<JobCandidateListItemDto>>>
{
    private readonly JobCandidateScoringService _scoring = new();

    public async Task<IResult<PaginatedResult<JobCandidateListItemDto>>> Handle(
        GetJobCandidatesQuery request,
        CancellationToken ct)
    {
        var job = await LoadJobAsync(request.JobId, ct);
        if (job == null)
            return Result.Fail<PaginatedResult<JobCandidateListItemDto>>(JobMessages.JobNotFound);

        var targetCount = GetTargetCount(job);

        var req = await JobRequirementsService.GetAsync(unitOfWork, job, ct);

        var baseQuery = JobCandidatesQueryBuilder.BuildEligibleQuery(unitOfWork, job, req, request.Filter);

        var windowSize = Math.Max(targetCount * 10, request.Pagination.PageSize * 10);
        var window = await baseQuery.OrderByDescending(x => x.CreatedDate).Take(windowSize).ToListAsync(ct);

        if (window.Count == 0)
            return Result.Ok(new PaginatedResult<JobCandidateListItemDto>([], 0, request.Pagination.PageNumber, request.Pagination.PageSize));

        var ids = window.Select(x => x.ApplicantId).Distinct().ToList();
        var profiles = await CandidateProfileLoader.LoadForScoringAsync(unitOfWork, ids, ct);

        var scored = _scoring.Score(window, profiles, job, req);

        if (request.Filter?.MinimumPoints is not null)
            scored = scored.Where(x => x.Points >= request.Filter.MinimumPoints.Value).ToList();

        var sorted = scored.OrderByDescending(x => x.Points).ThenByDescending(x => x.CreatedDate).ToList();

        var settings = await unitOfWork.GetEntityRepository<JobCandidateFilterSetting>().DbSet
            .AsNoTracking()
            .Include(s => s.CandidateTypePercentages)
            .Include(s => s.NationalityPercentages)
            .FirstOrDefaultAsync(s => s.JobId == request.JobId, ct);

        var finalList = JobCandidatesFilterProcessor.ApplyPercentageFilters(sorted, settings, targetCount);

        var pageNumber = request.Pagination.PageNumber;
        var pageSize = request.Pagination.PageSize;

        var paged = finalList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

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

        return Result.Ok(new PaginatedResult<JobCandidateListItemDto>(items, finalList.Count, pageNumber, pageSize));
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
