using Cortex.Mediator.Queries;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.DTOs;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.Queries;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.Services;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Application.Features.Operations.Employee.JobCandidates.Handlers.Queries;

public sealed class GetJobCandidatesOverviewQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetJobCandidatesOverviewQuery, IResult<JobCandidatesOverviewDto>>
{
    private readonly JobCandidatePointsCalculator _pointsCalculator = new();

    public async Task<IResult<JobCandidatesOverviewDto>> Handle(
        GetJobCandidatesOverviewQuery request,
        CancellationToken cancellationToken)
    {
        // Load job (need points + category + major/submajor for eligibility)
        var job = await unitOfWork.GetEntityRepository<Domain.Entities.Recruitment.Job>().DbSet
            .AsNoTracking()
            .Include(j => j.JobPoints).ThenInclude(p => p!.Details)
            .Include(j => j.JobCategory)
            .Include(j => j.Major)
            .Include(j => j.SubMajor)
            .FirstOrDefaultAsync(j => j.Id == request.JobId, cancellationToken);

        if (job is null)
            return Result.Fail<JobCandidatesOverviewDto>(JobMessages.JobNotFound);

        // Requirements (required skills from MajorSkill + job major/submajor ids)
        var req = await JobRequirementsService.GetAsync(unitOfWork, job, cancellationToken);

        // Fully eligible query (no duplicated checks here)
        var baseQuery = JobCandidatesQueryBuilder.BuildEligibleQuery(
            unitOfWork,
            job,
            req,
            request.ToFilter());

        var targetCount = GetTargetCount(job);
        var windowSize = Math.Max(targetCount * 10, 500);

        var window = await baseQuery
            .OrderByDescending(c => c.CreatedDate)
            .Take(windowSize)
            .ToListAsync(cancellationToken);

        if (window.Count == 0)
            return Result.Ok(new JobCandidatesOverviewDto());

        // Load heavy profiles for scoring
        var ids = window.Select(x => x.ApplicantId).Distinct().ToList();
        var profiles = await CandidateProfileLoader.LoadForScoringAsync(unitOfWork, ids, cancellationToken);
        var profileMap = profiles.ToDictionary(p => p.UserId);

        // Score
        var pointsList = new List<int>(window.Count);
        foreach (var c in window)
        {
            if (!profileMap.TryGetValue(c.ApplicantId, out var p))
                continue;

            // Latest major for display/scoring
            var major = p.Qualifications?
                .OrderByDescending(q => q.GraduationYear)
                .Select(q => q.Major)
                .FirstOrDefault();

            var candidate = c with { Applicant = p.User, Profile = p, Major = major };
            var points = _pointsCalculator.Calculate(candidate, job.JobPoints);
            pointsList.Add(points);
        }

      
        
        var totalInvited = await unitOfWork
            .GetEntityRepository<Invitation>()
            .DbSet
            .CountAsync(i => i.JobId == job.Id, cancellationToken: cancellationToken);
        var totalEligible = pointsList.Count;
        var abovePoints = pointsList.Count(x => x >= 800);
        var avg = totalEligible == 0 ? 0 : pointsList.Average();

        return Result.Ok(new JobCandidatesOverviewDto
        {
            TotalCandidatesCount = totalInvited,
            AvailableCandidatesCount = totalEligible,
            AbovePointsCandidatesCount = abovePoints,
            PointsAverage = Math.Round(avg, 2)
        });
    }

    private static int GetTargetCount(Domain.Entities.Recruitment.Job job)
    {
        var vacancies = Math.Max(1, job.NumberOfVacancies);
        return job.JobCategoryId == JobCategoryIds.Academic ? vacancies * 5 : vacancies;
    }
}
